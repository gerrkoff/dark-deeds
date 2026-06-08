import { delay } from '../../common/utils/delay'
import { taskApi, TaskApi } from '../api/TaskApi'
import { TaskModel } from '../models/TaskModel'
import { TaskVersionModel } from '../models/TaskVersionModel'

export type StatusUpdateSubscription = (isSynchronizing: boolean) => void
export type SaveFinishSubscription = (
    notSaved: number,
    savedTasks: TaskVersionModel[],
    conflictedTasks: TaskModel[],
) => void

export interface OnlineUpdateResult {
    tasksConflicted: TaskModel[]
    tasksToApply: TaskModel[]
}

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
            const tasksInFlightCount = this.tasksInFlight.size
            this.tasksToSave = new Map<string, TaskModel>()
            let failed = false

            let savedTasks: TaskModel[] = []

            try {
                savedTasks = await this.taskApi.saveTasks([...this.tasksInFlight.values()])
            } catch (error) {
                console.error('Failed to save tasks:', error)
                failed = true
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

            // On HTTP success, tasks still in flight were rejected by the backend on a version
            // conflict. Report only the ones that are NOT queued again in tasksToSave: their
            // change was neither saved nor is pending a retry, and no newer version arrived to
            // reconcile it (otherwise the hub update would have removed them from tasksInFlight).
            // They are dropped; reconciliation comes later via the hub or a reload on reconnect.
            const conflictedTasks = failed
                ? []
                : [...this.tasksInFlight.values()].filter(task => !this.tasksToSave.has(task.uid))

            // Two reporting channels: `notSaved` counts a transport failure (whole batch threw,
            // will be retried), while `conflictedTasks` carries lost updates on an HTTP success.
            // On the transport-failure branch savedTasks is always empty, so the count is simply
            // the in-flight size.
            this.saveFinishSubscriptions.forEach(x =>
                x(
                    failed ? tasksInFlightCount : 0,
                    savedTasks.map(task => ({ uid: task.uid, version: task.version })),
                    conflictedTasks,
                ),
            )

            if (failed) {
                // Transport error - re-queue everything still in flight and retry after a delay.
                for (const [uid, task] of this.tasksInFlight) {
                    if (!this.tasksToSave.has(uid)) {
                        this.tasksToSave.set(uid, task)
                    }
                }

                await delay(5000)
            }
        }
    }

    processTasksOnlineUpdate(updatedTasks: TaskModel[]): OnlineUpdateResult {
        const tasksConflicted: TaskModel[] = []
        const tasksToApply: TaskModel[] = []

        for (const updatedTask of updatedTasks) {
            const pendingTask = this.tasksToSave.get(updatedTask.uid) ?? this.tasksInFlight.get(updatedTask.uid)

            if (pendingTask === undefined) {
                // Not pending locally - apply the incoming snapshot as-is.
                tasksToApply.push(updatedTask)
                continue
            }

            if (updatedTask.version > pendingTask.version) {
                // Backend has a newer version than our pending edit - backend wins: drop the
                // local edit, report the conflict and overwrite local state.
                this.tasksToSave.delete(updatedTask.uid)
                this.tasksInFlight.delete(updatedTask.uid)
                tasksConflicted.push(updatedTask)
                tasksToApply.push(updatedTask)
                continue
            }

            // Our pending edit is as new as the incoming snapshot (same version) - keep the
            // local edit and skip the incoming task so the unsaved change is not reverted.
        }

        return { tasksConflicted, tasksToApply }
    }
}

export const taskSyncService = new TaskSyncService(taskApi)
