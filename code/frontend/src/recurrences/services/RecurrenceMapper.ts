import { dateService, DateService } from '../../common/services/DateService'
import { PlannedRecurrenceDto } from '../models/PlannedRecurrenceDto'
import { PlannedRecurrenceModel } from '../models/PlannedRecurrenceModel'

export class RecurrenceMapper {
    constructor(private dateService: DateService) {}

    mapToDto(items: PlannedRecurrenceModel[]): PlannedRecurrenceDto[] {
        return items.map(x => ({
            uid: x.uid,
            task: x.task,
            startDate: this.dateService.changeFromLocalToUtc(new Date(x.startDate)),
            endDate: x.endDate ? this.dateService.changeFromLocalToUtc(new Date(x.endDate)) : null,
            everyNthDay: x.everyNthDay,
            everyMonthDay: x.everyMonthDay,
            everyWeekday: x.everyWeekday,
            isDeleted: x.isDeleted,
        }))
    }

    mapToModel(items: PlannedRecurrenceDto[]): PlannedRecurrenceModel[] {
        return items.map(x => ({
            uid: x.uid,
            task: x.task,
            startDate: this.dateService.changeFromUtcToLocal(x.startDate).valueOf(),
            endDate: x.endDate ? this.dateService.changeFromUtcToLocal(x.endDate).valueOf() : null,
            everyNthDay: x.everyNthDay,
            everyMonthDay: x.everyMonthDay,
            everyWeekday: x.everyWeekday,
            isDeleted: x.isDeleted,
        }))
    }
}

export const recurrenceMapper = new RecurrenceMapper(dateService)
