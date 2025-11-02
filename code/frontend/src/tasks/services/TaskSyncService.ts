import { delay } from '../../common/utils/delay'
import { taskApi, TaskApi } from '../api/TaskApi'
import { TaskModel } from '../models/TaskModel'
import { TaskVersionModel } from '../models/TaskVersionModel'

export type StatusUpdateSubscription = (isSynchronizing: boolean) => void
export type SaveFinishSubscription = (notSaved: number, savedTasks: TaskVersionModel[]) => void

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
            this.tasksToSave.set(task.uid, { ...task })
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

            // Update versions in tasksToSave if task was modified while in flight
            for (const savedTask of savedTasks) {
                const taskToSave = this.tasksToSave.get(savedTask.uid)
                if (taskToSave) {
                    this.tasksToSave.set(savedTask.uid, {
                        ...taskToSave,
                        version: savedTask.version,
                    })
                }
            }

            for (const task of savedTasks) {
                this.tasksInFlight.delete(task.uid)
            }

            this.saveFinishSubscriptions.forEach(x =>
                x(
                    this.tasksInFlight.size,
                    savedTasks.map(task => ({ uid: task.uid, version: task.version })),
                ),
            )

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

    processTasksOnlineUpdate(updatedTasks: TaskModel[]): TaskModel[] {
        const tasksConflicted: TaskModel[] = []

        for (const updatedTask of updatedTasks) {
            const taskToSave = this.tasksToSave.get(updatedTask.uid)
            const taskInFlight = this.tasksInFlight.get(updatedTask.uid)

            // If task is in save queue and incoming version is newer - conflict!
            if (taskToSave && updatedTask.version > taskToSave.version) {
                this.tasksToSave.delete(updatedTask.uid)
                tasksConflicted.push(updatedTask)
                continue
            }

            // If task is in flight and incoming version is newer - conflict!
            if (taskInFlight && updatedTask.version > taskInFlight.version) {
                this.tasksInFlight.delete(updatedTask.uid)
                tasksConflicted.push(updatedTask)
                continue
            }
        }

        return tasksConflicted
    }
}

export const taskSyncService = new TaskSyncService(taskApi)
