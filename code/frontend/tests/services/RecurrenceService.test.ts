import { expect, test } from 'vitest'
import { RecurrenceService } from '../../src/recurrences/services/RecurrenceService'
import { DateService } from '../../src/common/services/DateService'
import { PlannedRecurrenceModel } from '../../src/recurrences/models/PlannedRecurrenceModel'
import { RecurrenceWeekdayEnum } from '../../src/recurrences/models/RecurrenceWeekdayEnum'

function service(): RecurrenceService {
    return new RecurrenceService(new DateService())
}

function recurrence(overrides: Partial<PlannedRecurrenceModel>): PlannedRecurrenceModel {
    return {
        uid: '',
        task: 'Task',
        startDate: new Date(2100, 0, 15).valueOf(),
        endDate: null,
        everyNthDay: null,
        everyMonthDay: null,
        everyWeekday: RecurrenceWeekdayEnum.Monday,
        isDeleted: false,
        ...overrides,
    }
}

test('[print] renders the from and until borders as dd/mm/yyyy', () => {
    const borders = service().print(
        recurrence({ startDate: new Date(2100, 0, 15).valueOf(), endDate: new Date(2100, 1, 20).valueOf() }),
    ).borders
    expect(borders).toBe('from 15/01/2100 until 20/02/2100')
})

test('[print] renders the until border as dd/mm/yyyy when the start date has passed', () => {
    const borders = service().print(
        recurrence({ startDate: new Date(2000, 0, 1).valueOf(), endDate: new Date(2100, 1, 20).valueOf() }),
    ).borders
    expect(borders).toBe('until 20/02/2100')
})
