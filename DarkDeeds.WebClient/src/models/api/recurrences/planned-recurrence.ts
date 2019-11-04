import { RecurrenceWeekdayEnum } from '../..'

export class PlannedRecurrence {
    constructor(
        public id: number,
        public task: string,
        public startDate: Date,
        public endDate: Date | null,
        public everyNthDay: number | null,
        public everyMonthDay: string,
        public everyWeekday: RecurrenceWeekdayEnum | null
    ) {}
}
