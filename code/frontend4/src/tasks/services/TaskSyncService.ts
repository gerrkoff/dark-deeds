import { taskApi, TaskApi } from '../api/TaskApi'
import { TaskModel } from '../models/TaskModel'
import { TaskVersionModel } from '../models/TaskVersionModel'

export type StatusUpdateSubscription = (isSynchronizing: boolean) => void

export type TaskVersionUpdateSubscription = (
    versions: TaskVersionModel[],
) => void

export class TaskSyncService {
    constructor(private taskApi: TaskApi) {}

    savingTasksPromise = new Promise<void>(r => r())
    tasksToSave = new Map<string, TaskModel>()
    isScheduled = false
    isSaving = false

    inProgress = false

    statusUpdateSubscriptions: StatusUpdateSubscription[] = []

    versionUpdateSubscriptions: TaskVersionUpdateSubscription[] = []

    subscribeStatusUpdate(callback: StatusUpdateSubscription) {
        this.statusUpdateSubscriptions.push(callback)
    }

    unsubscribeStatusUpdate(callback: StatusUpdateSubscription) {
        this.statusUpdateSubscriptions = this.statusUpdateSubscriptions.filter(
            x => x !== callback,
        )
    }

    subscribeVersionsUpdate(callback: TaskVersionUpdateSubscription) {
        this.versionUpdateSubscriptions.push(callback)
    }

    unsubscribeVersionsUpdate(callback: TaskVersionUpdateSubscription) {
        this.versionUpdateSubscriptions =
            this.versionUpdateSubscriptions.filter(x => x !== callback)
    }

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
        this.statusUpdateSubscriptions.forEach(x => x(true))

        await this.saveTasks()

        this.statusUpdateSubscriptions.forEach(x => x(false))
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

            this.notifyVersionUpdate(savedTasks)
        }
    }

    private notifyVersionUpdate(newTasks: TaskModel[]) {
        newTasks.forEach(x => {
            const task = this.tasksToSave.get(x.uid)

            if (task) {
                this.tasksToSave.set(task.uid, { ...task, version: x.version })
            }
        })

        const versions = newTasks.map(x => ({
            uid: x.uid,
            version: x.version,
        }))

        this.versionUpdateSubscriptions.forEach(x => x(versions))
    }

    private concat(
        tasks1: Map<string, TaskModel>,
        tasks2: Map<string, TaskModel>,
    ): Map<string, TaskModel> {
        return new Map<string, TaskModel>([...tasks1, ...tasks2])
    }
}

export const taskSyncService = new TaskSyncService(taskApi)
