import { taskApi, TaskApi } from '../api/TaskApi'
import { TaskModel } from '../models/TaskModel'
import { TaskVersionModel } from '../models/TaskVersionModel'
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
    tasksInFlight = new Map<string, TaskModel>()

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
            this.tasksInFlight = this.tasksToSave
            this.tasksToSave = new Map<string, TaskModel>()

            const savedTasks = await this.taskApi.saveTasks([
                ...this.tasksInFlight.values(),
            ])

            for (const task of savedTasks) {
                this.tasksInFlight.delete(task.uid)
            }

            for (const [uid, task] of this.tasksInFlight) {
                if (!this.tasksToSave.has(uid)) {
                    this.tasksToSave.set(uid, task)
                }
            }
        }
    }

    updateTasks(updatedTasks: TaskModel[]): {
        tasksToNotify: TaskModel[]
        versionsToNotify: TaskVersionModel[]
    } {
        const tasksToNotify: TaskModel[] = []
        const versionsToNotify: TaskVersionModel[] = []

        for (const updatedTask of updatedTasks) {
            const taskToSave = this.tasksToSave.get(updatedTask.uid)
            const taskInFlight = this.tasksInFlight.get(updatedTask.uid)

            if (taskToSave) {
                if (updatedTask.version > taskToSave.version) {
                    taskToSave.version = updatedTask.version
                    versionsToNotify.push({
                        uid: updatedTask.uid,
                        version: updatedTask.version,
                    })
                }
            } else if (taskInFlight) {
                if (updatedTask.version > taskInFlight.version) {
                    taskInFlight.version = updatedTask.version
                    versionsToNotify.push({
                        uid: updatedTask.uid,
                        version: updatedTask.version,
                    })
                }
            } else {
                tasksToNotify.push(updatedTask)
            }
        }

        return {
            tasksToNotify,
            versionsToNotify,
        }
    }
}

export const taskSyncService = new TaskSyncService(
    taskApi,
    taskSubscriptionService,
)
