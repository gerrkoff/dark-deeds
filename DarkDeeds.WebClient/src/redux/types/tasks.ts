import { Task } from '../../models'

export interface ITasksState {
    loading: boolean
    loaded: boolean
    saving: boolean
    notSaved: boolean
    tasks: Task[]
}
