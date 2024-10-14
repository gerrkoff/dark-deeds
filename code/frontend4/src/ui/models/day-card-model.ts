import { TaskEntity } from '../../common/models/task-entity'

export interface DayCardModel {
    date: Date
    tasks: TaskEntity[]
}
