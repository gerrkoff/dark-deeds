import { taskApi, TaskApi } from '../api/TaskApi'
import { TaskModel } from '../models/TaskModel'

export type ProcessUpdateCallback = (isPending: boolean) => void

export class TaskSavingService {
    private savingTasksPromise = new Promise<void>(r => r())
    private tasksToSave: TaskModel[] = []
    private isScheduled = false
    private isSaving = false
    private processUpdateSubscriptions: ProcessUpdateCallback[] = []

    constructor(private taskApi: TaskApi) {}

    subscribeProcessUpdate(callback: ProcessUpdateCallback) {
        this.processUpdateSubscriptions.push(callback)
    }

    unsubscribeProcessUpdate(callback: ProcessUpdateCallback) {
        this.processUpdateSubscriptions =
            this.processUpdateSubscriptions.filter(x => x !== callback)
    }

    scheduleSaving(tasks: TaskModel[]) {
        this.tasksToSave.push(...tasks)

        if (!this.isScheduled) {
            this.savingTasksPromise = this.schedule()
        }
    }

    private async schedule() {
        if (!this.isSaving) {
            this.processUpdateSubscriptions.forEach(x => x(true))
        }

        this.isScheduled = true
        await this.savingTasksPromise
        this.isScheduled = false
        const tasks = this.tasksToSave
        this.tasksToSave = []
        this.isSaving = true
        await this.save(tasks)
        this.isSaving = false

        if (!this.isScheduled) {
            this.processUpdateSubscriptions.forEach(x => x(false))
        }
    }

    private async save(tasks: TaskModel[]): Promise<void> {
        await this.taskApi.saveTasks(tasks)
    }
}

export const taskSaveService = new TaskSavingService(taskApi)
