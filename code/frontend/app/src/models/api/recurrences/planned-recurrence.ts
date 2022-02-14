import { IDateable,RecurrenceWeekdayEnum } from 'models'

export class PlannedRecurrence implements IDateable {
    constructor(
        public uid: string,
        public task: string,
        public startDate: Date,
        public endDate: Date | null,
        public everyNthDay: number | null,
        public everyMonthDay: string | null,
        public everyWeekday: RecurrenceWeekdayEnum | null,
        public isDeleted: boolean,
        public isLocal: boolean = false
    ) {}
}
