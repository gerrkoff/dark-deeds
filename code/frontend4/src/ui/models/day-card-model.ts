import { TaskEntity } from '../../common/models/TaskEntity'

export interface DayCardModel {
    date: Date
    tasks: TaskEntity[]
}
