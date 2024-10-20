import { TaskEntity } from '../../tasks/models/TaskEntity'
import { DayCardModel } from '../../ui/models/DayCardModel'

export interface OverviewModel {
    noDate: TaskEntity[]
    expired: DayCardModel[]
    current: DayCardModel[]
    future: DayCardModel[]
}
