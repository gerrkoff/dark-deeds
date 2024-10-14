import { TaskEntity } from '../entities/task-entity'

export interface DayCardModel {
    date: Date
    tasks: TaskEntity[]
}
