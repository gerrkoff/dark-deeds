import { injectable, inject } from 'inversify'
import { PlannedRecurrence, RecurrenceWeekdayEnum } from '../../models'
import { DateService } from '..'
import diToken from '../token'

@injectable()
export class RecurrenceService {

    public constructor(
        @inject(diToken.DateService) private dateService: DateService
    ) {}

    public print(recurrence: PlannedRecurrence): string {
        let result = `${recurrence.task}, `

        // Monthly
        if (recurrence.everyMonthDay !== null &&
            recurrence.everyNthDay === null &&
            recurrence.everyWeekday === null) {

            result += `monthly on ${this.printMonthDays(recurrence.everyMonthDay)}`
            return result
        }

        // Weekly
        if (recurrence.everyWeekday !== null &&
            recurrence.everyNthDay === null &&
            recurrence.everyMonthDay === null) {

            const weekdayList = this.evalWeekdayList(recurrence.everyWeekday)

            if (weekdayList.length === 5 &&
                weekdayList.indexOf(1) >= -1 &&
                weekdayList.indexOf(2) >= -1 &&
                weekdayList.indexOf(3) >= -1 &&
                weekdayList.indexOf(4) >= -1 &&
                weekdayList.indexOf(5) >= -1) {

                result += 'every weekday (Monday to Friday)'
            } else {
                result += `weekly on ${this.printWeekDays(weekdayList)}`
            }

            return result
        }

        // Daily
        if (recurrence.everyNthDay !== null &&
            recurrence.everyMonthDay === null &&
            recurrence.everyWeekday === null) {

            result += `daily`

            return result
        }

        if (recurrence.everyNthDay !== null) {
            result += this.printNthDays(recurrence.everyNthDay)
        }

        return result
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

        // tslint:no-bitwise
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
