import { TaskModel } from '../../tasks/models/TaskModel'

export interface DayCardModel {
    date: Date
    tasks: TaskModel[]
}
