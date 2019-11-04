import { RecurrenceWeekdayEnum } from '../..'

export class PlannedRecurrence {
    constructor(
        public id: number,
        public task: string,
        public startDate: Date,
        public endDate: Date | null,
        public everyNthDay: number | null,
        public everyMonthDay: string | null,
        public everyWeekday: RecurrenceWeekdayEnum | null
    ) {}
}

// Some task

    // Monthly on 1, 2, 3, 10 date(s)
    // Every weekday (Monday to Friday)
    // Weekly on Monday, Tuesday, Friday
    // Daily
// Every 2nd day
// Every 3rd day
// Every Nth day on Monday, Tuesday and 1, 2, 3, 10 dates

// from 1/1/2010 untill 1/1/2010
