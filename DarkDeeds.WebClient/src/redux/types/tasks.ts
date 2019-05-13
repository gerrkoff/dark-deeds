import { Task, TaskLoadingStateEnum } from '../../models'

export interface ITasksState {
    loadingState: TaskLoadingStateEnum
    saving: boolean
    changed: boolean
    tasks: Task[]
    hubReconnecting: boolean
}
