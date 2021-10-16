import { RecurrenceWeekdayEnum, IDateable } from '../..'

export class PlannedRecurrence implements IDateable {
    constructor(
        public id: number,
        public task: string,
        public startDate: Date,
        public endDate: Date | null,
        public everyNthDay: number | null,
        public everyMonthDay: string | null,
        public everyWeekday: RecurrenceWeekdayEnum | null,
        public isDeleted: boolean
    ) {}
}
