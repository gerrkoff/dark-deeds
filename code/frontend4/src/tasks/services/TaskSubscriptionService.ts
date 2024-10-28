import { TaskModel } from '../models/TaskModel'

export type StatusUpdateSubscription = (isSynchronizing: boolean) => void

export type TaskUpdateSubscription = (tasks: TaskModel[]) => void

export class TaskSubscriptionService {
    private statusUpdateSubscriptions: StatusUpdateSubscription[] = []
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
