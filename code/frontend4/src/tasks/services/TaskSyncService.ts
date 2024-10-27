import { taskApi, TaskApi } from '../api/TaskApi'
import { TaskModel } from '../models/TaskModel'

export type TaskSyncServiceSubscription = (isSynchronizing: boolean) => void

export class TaskSyncService {
    constructor(private taskApi: TaskApi) {}

    savingTasksPromise = new Promise<void>(r => r())
    tasksToSave: TaskModel[] = []
    isScheduled = false
    isSaving = false

    subscriptions: TaskSyncServiceSubscription[] = []

    subscribe(callback: TaskSyncServiceSubscription) {
        this.subscriptions.push(callback)
    }

    unsubscribe(callback: TaskSyncServiceSubscription) {
        this.subscriptions = this.subscriptions.filter(x => x !== callback)
    }

    sync(tasks: TaskModel[]) {
        this.tasksToSave = this.appendAndFlatten(this.tasksToSave, tasks)

        if (!this.isScheduled) {
            this.savingTasksPromise = this.schedule()
        }
    }

    private async schedule(): Promise<void> {
        if (!this.isSaving) {
            this.subscriptions.forEach(x => x(true))
        }

        this.isScheduled = true
        await this.savingTasksPromise
        this.isScheduled = false
        const tasks = this.tasksToSave
        this.tasksToSave = []
        this.isSaving = true
        try {
            await this.taskApi.saveTasks(tasks)
        } catch {
            this.tasksToSave = this.prependAndFlatten(this.tasksToSave, tasks)
            this.savingTasksPromise = this.schedule()
            return
        }
        this.isSaving = false

        if (!this.isScheduled) {
            this.subscriptions.forEach(x => x(false))
        }
    }

    private appendAndFlatten(
        tasks: TaskModel[],
        tasksToAdd: TaskModel[],
    ): TaskModel[] {
        const newTasks = tasks.filter(
            task => !tasksToAdd.some(t => t.uid === task.uid),
        )
        newTasks.push(...tasksToAdd)
        return newTasks
    }

    private prependAndFlatten(
        tasks: TaskModel[],
        tasksToAdd: TaskModel[],
    ): TaskModel[] {
        const newTasks = tasksToAdd.filter(
            task => !tasks.some(t => t.uid === task.uid),
        )
        newTasks.push(...tasks)
        return newTasks
    }
}

export const taskSyncService = new TaskSyncService(taskApi)
