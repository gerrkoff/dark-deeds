import { RecurrenceWeekdayEnum } from './RecurrenceWeekdayEnum'

export interface PlannedRecurrenceDto {
    uid: string
    task: string
    startDate: Date
    endDate: Date | null
    everyNthDay: number | null
    everyMonthDay: string | null
    everyWeekday: RecurrenceWeekdayEnum | null
    isDeleted: boolean
}
