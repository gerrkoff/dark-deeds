import { RecurrenceWeekdayEnum } from './RecurrenceWeekdayEnum'

export interface PlannedRecurrenceModel {
    uid: string
    task: string
    startDate: number
    endDate: number | null
    everyNthDay: number | null
    everyMonthDay: string | null
    everyWeekday: RecurrenceWeekdayEnum | null
    isDeleted: boolean
}
