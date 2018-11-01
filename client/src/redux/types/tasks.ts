import { Task } from '../../models'

export interface ITasksState {
    loading: boolean,
    tasks: Task[]
}
