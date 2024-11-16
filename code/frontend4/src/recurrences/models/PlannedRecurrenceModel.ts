import { RecurrenceWeekdayEnum } from './RecurrenceWeekdayEnum'

export class PlannedRecurrenceModel {
    constructor(
        public uid: string,
        public task: string,
        public startDate: number,
        public endDate: number | null,
        public everyNthDay: number | null,
        public everyMonthDay: string | null,
        public everyWeekday: RecurrenceWeekdayEnum | null,
        public isDeleted: boolean,
    ) {}
}
