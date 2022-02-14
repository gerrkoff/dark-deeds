import {
    PlannedRecurrence,
    RecurrenceWeekdayEnum,
    PlannedRecurrencePrint,
} from '../../models'
import { DateService, dateService } from './date-service'

export class RecurrenceService {
    public constructor(private dateService: DateService) {}

    public print(recurrence: PlannedRecurrence): PlannedRecurrencePrint {
        const task = recurrence.task
        const repeatative = this.printRepeatativePart(recurrence)
        const borders = this.printBordersPart(recurrence)
        return new PlannedRecurrencePrint(task, repeatative, borders)
    }

    private printBordersPart(recurrence: PlannedRecurrence): string {
        let result = ''
        const today = this.dateService.today()
        if (today < recurrence.startDate) {
            result += `from ${this.printDate(recurrence.startDate)}`

            if (recurrence.endDate !== null) {
                result += ' '
            }
        }

        if (recurrence.endDate !== null) {
            result += `untill ${this.printDate(recurrence.endDate)}`
        }

        return result
    }

    private printRepeatativePart(recurrence: PlannedRecurrence): string {
        // Monthly
        if (
            recurrence.everyMonthDay !== null &&
            recurrence.everyNthDay === null &&
            recurrence.everyWeekday === null
        ) {
            return `monthly on ${this.printMonthDays(recurrence.everyMonthDay)}`
        }

        // Weekly
        if (
            recurrence.everyWeekday !== null &&
            recurrence.everyNthDay === null &&
            recurrence.everyMonthDay === null
        ) {
            const weekdayList = this.evalWeekdayList(recurrence.everyWeekday)

            if (
                weekdayList.length === 5 &&
                weekdayList.indexOf(1) >= -1 &&
                weekdayList.indexOf(2) >= -1 &&
                weekdayList.indexOf(3) >= -1 &&
                weekdayList.indexOf(4) >= -1 &&
                weekdayList.indexOf(5) >= -1
            ) {
                return 'every weekday (Monday to Friday)'
            } else {
                return `weekly on ${this.printWeekDays(weekdayList)}`
            }
        }

        // Daily
        if (
            recurrence.everyNthDay === 1 &&
            recurrence.everyMonthDay === null &&
            recurrence.everyWeekday === null
        ) {
            return `daily`
        }

        let result = ''
        if (recurrence.everyNthDay !== null) {
            result += this.printNthDays(recurrence.everyNthDay)

            if (
                recurrence.everyMonthDay !== null ||
                recurrence.everyWeekday !== null
            ) {
                result += ' '
            }
        }

        if (
            recurrence.everyMonthDay !== null ||
            recurrence.everyWeekday !== null
        ) {
            result += 'on '

            if (recurrence.everyWeekday !== null) {
                const weekdayList = this.evalWeekdayList(
                    recurrence.everyWeekday
                )
                result += this.printWeekDays(weekdayList)

                if (recurrence.everyMonthDay !== null) {
                    result += ' and '
                }
            }

            if (recurrence.everyMonthDay !== null) {
                result += this.printMonthDays(recurrence.everyMonthDay)
            }
        }

        return result === '' ? 'no repeats' : result
    }

    private printDate(date: Date): string {
        return date.toLocaleDateString('en-US')
    }

    private printNthDays(nthDays: number): string {
        if (nthDays === 1) {
            return 'every 1st day'
        }
        if (nthDays === 2) {
            return 'every 2nd day'
        }
        if (nthDays === 3) {
            return 'every 3rd day'
        }
        return `every ${nthDays}th day`
    }

    private printMonthDays(monthDays: string): string {
        const days = monthDays.split(',')
        const daysString = days.join(', ')
        let result = `${daysString} date`
        if (days.length > 1) {
            result += 's'
        }
        return result
    }

    private printWeekDays(weekdayList: number[]): string {
        const days = weekdayList.map(x => this.dateService.daysLong[x])
        const daysString = days.join(', ')
        return `${daysString}`
    }

    private evalWeekdayList(weekdays: RecurrenceWeekdayEnum): number[] {
        const list: number[] = []

        if (weekdays & RecurrenceWeekdayEnum.Monday) {
            list.push(1)
        }
        if (weekdays & RecurrenceWeekdayEnum.Tuesday) {
            list.push(2)
        }
        if (weekdays & RecurrenceWeekdayEnum.Wednesday) {
            list.push(3)
        }
        if (weekdays & RecurrenceWeekdayEnum.Thursday) {
            list.push(4)
        }
        if (weekdays & RecurrenceWeekdayEnum.Friday) {
            list.push(5)
        }
        if (weekdays & RecurrenceWeekdayEnum.Saturday) {
            list.push(6)
        }
        if (weekdays & RecurrenceWeekdayEnum.Sunday) {
            list.push(0)
        }

        return list
    }
}

export const recurrenceService = new RecurrenceService(dateService)
