import { Task } from '../../models'

export interface ITasksState {
    loading: boolean
    saving: boolean
    notSaved: boolean
    tasks: Task[]
}
