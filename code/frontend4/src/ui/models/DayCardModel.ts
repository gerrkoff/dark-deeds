import { TaskEntity } from '../../tasks/models/TaskEntity'

export interface DayCardModel {
    date: Date
    tasks: TaskEntity[]
}
