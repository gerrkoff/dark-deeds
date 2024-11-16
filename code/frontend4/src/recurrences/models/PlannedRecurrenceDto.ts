import { RecurrenceWeekdayEnum } from './RecurrenceWeekdayEnum'

export class PlannedRecurrenceDto {
    constructor(
        public uid: string,
        public task: string,
        public startDate: Date,
        public endDate: Date | null,
        public everyNthDay: number | null,
        public everyMonthDay: string | null,
        public everyWeekday: RecurrenceWeekdayEnum | null,
        public isDeleted: boolean,
    ) {}
}
