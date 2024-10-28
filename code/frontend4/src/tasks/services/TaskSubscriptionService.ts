import { TaskModel } from '../models/TaskModel'
import { TaskVersionModel } from '../models/TaskVersionModel'

export type StatusUpdateSubscription = (isSynchronizing: boolean) => void

export type TaskVersionUpdateSubscription = (
    versions: TaskVersionModel[],
) => void

export type TaskUpdateSubscription = (tasks: TaskModel[]) => void

export class TaskSubscriptionService {
    private statusUpdateSubscriptions: StatusUpdateSubscription[] = []
    private versionUpdateSubscriptions: TaskVersionUpdateSubscription[] = []
    private taskUpdateSubscriptions: TaskUpdateSubscription[] = []

    subscribeStatusUpdate(callback: StatusUpdateSubscription) {
        this.statusUpdateSubscriptions.push(callback)
    }

    unsubscribeStatusUpdate(callback: StatusUpdateSubscription) {
        this.statusUpdateSubscriptions = this.statusUpdateSubscriptions.filter(
            x => x !== callback,
        )
    }

    notifyStatusUpdate(isSynchronizing: boolean) {
        this.statusUpdateSubscriptions.forEach(x => x(isSynchronizing))
    }

    subscribeVersionsUpdate(callback: TaskVersionUpdateSubscription) {
        this.versionUpdateSubscriptions.push(callback)
    }

    unsubscribeVersionsUpdate(callback: TaskVersionUpdateSubscription) {
        this.versionUpdateSubscriptions =
            this.versionUpdateSubscriptions.filter(x => x !== callback)
    }

    notifyVersionsUpdate(versions: TaskVersionModel[]) {
        this.versionUpdateSubscriptions.forEach(x => x(versions))
    }

    subscribeTaskUpdate(callback: TaskUpdateSubscription) {
        this.taskUpdateSubscriptions.push(callback)
    }

    unsubscribeTaskUpdate(callback: TaskUpdateSubscription) {
        this.taskUpdateSubscriptions = this.taskUpdateSubscriptions.filter(
            x => x !== callback,
        )
    }

    notifyTaskUpdate(tasks: TaskModel[]) {
        this.taskUpdateSubscriptions.forEach(x => x(tasks))
    }
}

export const taskSubscriptionService = new TaskSubscriptionService()
