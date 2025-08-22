import { TaskModel } from '../../tasks/models/TaskModel'
import { DayCardModel } from '../../day-card/models/DayCardModel'

export interface OverviewModel {
    noDate: TaskModel[]
    weekly: TaskModel[]
    expired: DayCardModel[]
    current: DayCardModel[]
    future: DayCardModel[]
}
