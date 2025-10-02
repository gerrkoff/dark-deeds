import { delay } from '../../common/utils/delay'
import { taskApi, TaskApi } from '../api/TaskApi'
import { TaskModel } from '../models/TaskModel'
import { TaskVersionModel } from '../models/TaskVersionModel'

export type StatusUpdateSubscription = (isSynchronizing: boolean) => void
export type SaveFinishSubscription = (notSaved: number) => void

export class TaskSyncService {
    constructor(private taskApi: TaskApi) {}

    private statusUpdateSubscriptions: StatusUpdateSubscription[] = []

    subscribeStatusUpdate(callback: StatusUpdateSubscription) {
        this.statusUpdateSubscriptions.push(callback)
    }

    unsubscribeStatusUpdate(callback: StatusUpdateSubscription) {
        this.statusUpdateSubscriptions = this.statusUpdateSubscriptions.filter(x => x !== callback)
    }

    private saveFinishSubscriptions: SaveFinishSubscription[] = []

    subscribeSaveFinish(callback: SaveFinishSubscription) {
        this.saveFinishSubscriptions.push(callback)
    }

    unsubscribeSaveFinish(callback: SaveFinishSubscription) {
        this.saveFinishSubscriptions = this.saveFinishSubscriptions.filter(x => x !== callback)
    }

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
        this.statusUpdateSubscriptions.forEach(x => x(true))

        await this.saveTasks()

        this.statusUpdateSubscriptions.forEach(x => x(false))
        this.inProgress = false
    }

    private async saveTasks(): Promise<void> {
        while (this.tasksToSave.size > 0) {
            this.tasksInFlight = this.tasksToSave
            this.tasksToSave = new Map<string, TaskModel>()
            let wait = false

            let savedTasks: TaskModel[] = []

            try {
                savedTasks = await this.taskApi.saveTasks([...this.tasksInFlight.values()])
            } catch (error) {
                console.error('Failed to save tasks:', error)
                wait = true
            }

            for (const task of savedTasks) {
                this.tasksInFlight.delete(task.uid)
            }

            this.saveFinishSubscriptions.forEach(x => x(this.tasksInFlight.size))

            for (const [uid, task] of this.tasksInFlight) {
                if (!this.tasksToSave.has(uid)) {
                    this.tasksToSave.set(uid, task)
                }
            }

            if (wait) {
                await delay(5000)
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

            if (taskToSave && updatedTask.version > taskToSave.version) {
                taskToSave.version = updatedTask.version
                versionsToNotify.push({
                    uid: updatedTask.uid,
                    version: updatedTask.version,
                })
            }

            if (taskInFlight && updatedTask.version > taskInFlight.version) {
                taskInFlight.version = updatedTask.version
                versionsToNotify.push({
                    uid: updatedTask.uid,
                    version: updatedTask.version,
                })
            }

            if (!taskToSave && !taskInFlight) {
                tasksToNotify.push(updatedTask)
            }
        }

        return {
            tasksToNotify,
            versionsToNotify,
        }
    }
}

export const taskSyncService = new TaskSyncService(taskApi)
