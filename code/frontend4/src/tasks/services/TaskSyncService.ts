import { taskApi, TaskApi } from '../api/TaskApi'
import { TaskModel } from '../models/TaskModel'
import {
    taskSubscriptionService,
    TaskSubscriptionService,
} from './TaskSubscriptionService'

export class TaskSyncService {
    constructor(
        private taskApi: TaskApi,
        private taskSubscriptionService: TaskSubscriptionService,
    ) {}

    inProgress = false
    tasksToSave = new Map<string, TaskModel>()

    sync(tasks: TaskModel[]) {
        for (const task of tasks) {
            const taskToSave = this.tasksToSave.get(task.uid)
            const version = taskToSave ? taskToSave.version : task.version

            this.tasksToSave.set(task.uid, {
                ...task,
                version,
            })
        }

        this.schedule()
    }

    private async schedule(): Promise<void> {
        if (this.inProgress) {
            return
        }

        this.inProgress = true
        this.taskSubscriptionService.notifyStatusUpdate(true)

        await this.saveTasks()

        this.taskSubscriptionService.notifyStatusUpdate(false)
        this.inProgress = false
    }

    private async saveTasks(): Promise<void> {
        while (this.tasksToSave.size > 0) {
            const taskInFlight = this.tasksToSave
            this.tasksToSave = new Map<string, TaskModel>()

            const savedTasks = await this.taskApi.saveTasks([
                ...taskInFlight.values(),
            ])

            for (const task of savedTasks) {
                taskInFlight.delete(task.uid)
            }

            for (const [uid, task] of taskInFlight) {
                if (!this.tasksToSave.has(uid)) {
                    this.tasksToSave.set(uid, task)
                }
            }

            this.updateTasks(savedTasks)
        }
    }

    private updateTasks(updatedTasks: TaskModel[]) {
        const updateTasksToNotify: TaskModel[] = []

        for (const updatedTask of updatedTasks) {
            const taskToSave = this.tasksToSave.get(updatedTask.uid)

            if (taskToSave) {
                taskToSave.version = updatedTask.version
            } else {
                updateTasksToNotify.push(updatedTask)
            }
        }

        if (updateTasksToNotify.length > 0) {
            this.taskSubscriptionService.notifyTaskUpdate(updateTasksToNotify)
        }
    }
}

export const taskSyncService = new TaskSyncService(
    taskApi,
    taskSubscriptionService,
)
