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

            let savedTasks: TaskModel[] = []
            let failed = false

            try {
                savedTasks = await this.taskApi.saveTasks([...this.tasksInFlight.values()])
            } catch (error) {
                console.error('Failed to save tasks:', error)
                failed = true
            }

            if (failed) {
                // Transport error - nothing was saved. Report the failure count (drives the
                // "failed to save" toast), re-queue the whole in-flight batch and retry later.
                this.saveFinishSubscriptions.forEach(x => x(tasksInFlightCount, [], []))

                for (const [uid, task] of this.tasksInFlight) {
                    if (!this.tasksToSave.has(uid)) {
                        this.tasksToSave.set(uid, task)
                    }
                }

                await delay(5000)
                continue
            }

            // Update versions in tasksToSave if a saved task was modified again while in flight.
            for (const savedTask of savedTasks) {
                const taskToSave = this.tasksToSave.get(savedTask.uid)
                if (taskToSave) {
                    this.tasksToSave.set(savedTask.uid, {
                        ...taskToSave,
                        version: savedTask.version,
                    })
                }
            }

            // In-flight tasks the backend did not return were rejected on a version conflict.
            // Report those that were not saved and are not queued again in tasksToSave (no pending
            // re-edit): their change is lost and no newer version has arrived to reconcile it yet.
            // They are dropped; reconciliation comes later via the hub or a reload on reconnect.
            const savedUids = new Set(savedTasks.map(task => task.uid))
            const conflictedTasks = [...this.tasksInFlight.values()].filter(
                task => !savedUids.has(task.uid) && !this.tasksToSave.has(task.uid),
            )

            this.saveFinishSubscriptions.forEach(x =>
                x(
                    0,
                    savedTasks.map(task => ({ uid: task.uid, version: task.version })),
                    conflictedTasks,
                ),
            )

            // The in-flight batch is done: saved tasks persisted, dropped conflicts abandoned,
            // re-edits tracked in tasksToSave. Drain it so concurrent reloads/hub updates never
            // see stale entries as pending.
            this.tasksInFlight = new Map<string, TaskModel>()
        }
    }

    getPendingUids(): string[] {
        return [...new Set([...this.tasksToSave.keys(), ...this.tasksInFlight.keys()])]
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
