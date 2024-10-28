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

    sync(tasks: TaskModel[]) {
        const tasksMap = new Map<string, TaskModel>(tasks.map(x => [x.uid, x]))

        this.tasksToSave = this.concat(this.tasksToSave, tasksMap)

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

            savedTasks.forEach(x => {
                taskInFlight.delete(x.uid)
            })

            this.tasksToSave = this.concat(taskInFlight, this.tasksToSave)

            this.updateTasks(savedTasks)
        }
    }

    private updateTasks(updatedTasks: TaskModel[]) {
        const updateVersions: TaskVersionModel[] = []
        const updateTasks: TaskModel[] = []

        updatedTasks.forEach(x => {
            const task = this.tasksToSave.get(x.uid)

            if (task) {
                this.tasksToSave.set(task.uid, { ...task, version: x.version })
                updateVersions.push({ uid: x.uid, version: x.version })
            } else {
                updateTasks.push(x)
            }
        })

        // if (updateVersions.length > 0) {
        //     this.taskSubscriptionService.notifyVersionsUpdate(updateVersions)
        // }

        if (updateTasks.length > 0) {
            this.taskSubscriptionService.notifyTaskUpdate(updateTasks)
        }
    }

    private concat(
        tasks1: Map<string, TaskModel>,
        tasks2: Map<string, TaskModel>,
    ): Map<string, TaskModel> {
        return new Map<string, TaskModel>([...tasks1, ...tasks2])
    }
}

export const taskSyncService = new TaskSyncService(
    taskApi,
    taskSubscriptionService,
)
