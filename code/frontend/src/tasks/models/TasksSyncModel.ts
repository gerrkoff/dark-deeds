import { TaskModel } from './TaskModel'
import { TaskVersionModel } from './TaskVersionModel'

export interface TasksSyncModel {
    tasks: TaskModel[]
    versions: TaskVersionModel[]
}
