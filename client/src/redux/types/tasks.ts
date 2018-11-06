import { Task } from '../../models'

export interface ITasksState {
    loading: boolean,
    saving: boolean,
    tasks: Task[]
}
