import { DateService } from 'di/services/date-service'
import { RecurrenceService } from 'di/services/recurrence-service'
import { PlannedRecurrence, RecurrenceWeekdayEnum } from 'models'

function createService(date?: Date): RecurrenceService {
    if (date === undefined) {
        date = new Date(2019, 10, 7)
    }
    const dateServiceMock = {
        today: jest.fn().mockImplementation(() => date),
        daysLong: [
            'Sunday',
            'Monday',
            'Tuesday',
            'Wednesday',
            'Thursday',
            'Friday',
            'Saturday',
        ],
    }
    return new RecurrenceService(dateServiceMock as unknown as DateService)
}

test('[print] should return task title', async () => {
    const service = createService()

    const result = service.print(
        new PlannedRecurrence(
            '0',
            'Some task text',
            new Date(),
            null,
            null,
            null,
            null,
            false
        )
    )

    expect(result.task).toBe('Some task text')
})

test('[print] should return weekly', async () => {
    const service = createService()

    const result = service.print(
        new PlannedRecurrence(
            '0',
            '',
            new Date(),
            null,
            null,
            null,
            RecurrenceWeekdayEnum.Monday |
                RecurrenceWeekdayEnum.Saturday |
                RecurrenceWeekdayEnum.Sunday,
            false
        )
    )

    expect(result.repeatative).toBe('weekly on Monday, Saturday, Sunday')
})

test('[print] should return every weekday', async () => {
    const service = createService()

    const result = service.print(
        new PlannedRecurrence(
            '0',
            '',
            new Date(),
            null,
            null,
            null,
            RecurrenceWeekdayEnum.Monday |
                RecurrenceWeekdayEnum.Tuesday |
                RecurrenceWeekdayEnum.Wednesday |
                RecurrenceWeekdayEnum.Thursday |
                RecurrenceWeekdayEnum.Friday,
            false
        )
    )

    expect(result.repeatative).toBe('every weekday (Monday to Friday)')
})

test('[print] should return monthly', async () => {
    const service = createService()

    const result = service.print(
        new PlannedRecurrence(
            '0',
            '',
            new Date(),
            null,
            null,
            '1,3,10',
            null,
            false
        )
    )

    expect(result.repeatative).toBe('monthly on 1, 3, 10 dates')
})

test('[print] should return monthly (one date)', async () => {
    const service = createService()

    const result = service.print(
        new PlannedRecurrence('0', '', new Date(), null, null, '3', null, false)
    )

    expect(result.repeatative).toBe('monthly on 3 date')
})

test('[print] should return daily', async () => {
    const service = createService()

    const result = service.print(
        new PlannedRecurrence('0', '', new Date(), null, 1, null, null, false)
    )

    expect(result.repeatative).toBe('daily')
})

test('[print] should return every 2nd day', async () => {
    const service = createService()

    const result = service.print(
        new PlannedRecurrence('0', '', new Date(), null, 2, null, null, false)
    )

    expect(result.repeatative).toBe('every 2nd day')
})

test('[print] should return every 3rd day', async () => {
    const service = createService()

    const result = service.print(
        new PlannedRecurrence('0', '', new Date(), null, 3, null, null, false)
    )

    expect(result.repeatative).toBe('every 3rd day')
})

test('[print] should return every Nth day', async () => {
    const service = createService()

    const result = service.print(
        new PlannedRecurrence('0', '', new Date(), null, 10, null, null, false)
    )

    expect(result.repeatative).toBe('every 10th day')
})

test('[print] should return combo nth & weekday', async () => {
    const service = createService()

    const result = service.print(
        new PlannedRecurrence(
            '0',
            '',
            new Date(),
            null,
            1,
            null,
            RecurrenceWeekdayEnum.Monday,
            false
        )
    )

    expect(result.repeatative).toBe('every 1st day on Monday')
})

test('[print] should return combo nth & month day', async () => {
    const service = createService()

    const result = service.print(
        new PlannedRecurrence('0', '', new Date(), null, 2, '19', null, false)
    )

    expect(result.repeatative).toBe('every 2nd day on 19 date')
})

test('[print] should return combo nth & weekday & month day', async () => {
    const service = createService()

    const result = service.print(
        new PlannedRecurrence(
            '0',
            '',
            new Date(),
            null,
            3,
            '21,28',
            RecurrenceWeekdayEnum.Friday | RecurrenceWeekdayEnum.Saturday,
            false
        )
    )

    expect(result.repeatative).toBe(
        'every 3rd day on Friday, Saturday and 21, 28 dates'
    )
})

test('[print] should return combo weekday & month day', async () => {
    const service = createService()

    const result = service.print(
        new PlannedRecurrence(
            '0',
            '',
            new Date(),
            null,
            null,
            '13',
            RecurrenceWeekdayEnum.Friday,
            false
        )
    )

    expect(result.repeatative).toBe('on Friday and 13 date')
})

test('[print] should return from date if current date less', async () => {
    const service = createService(new Date(2010, 9, 9))

    const result = service.print(
        new PlannedRecurrence(
            '0',
            '',
            new Date(2010, 9, 10),
            null,
            null,
            null,
            null,
            false
        )
    )

    expect(result.borders).toBe('from 10/10/2010')
})

test('[print] should return nothing if current date equal to from', async () => {
    const service = createService(new Date(2010, 9, 10))

    const result = service.print(
        new PlannedRecurrence(
            '0',
            '',
            new Date(2010, 9, 10),
            null,
            null,
            null,
            null,
            false
        )
    )

    expect(result.borders).toBe('')
})

test('[print] should return untill date', async () => {
    const service = createService(new Date(2010, 9, 10))

    const result = service.print(
        new PlannedRecurrence(
            '0',
            '',
            new Date(2010, 9, 10),
            new Date(2010, 9, 10),
            null,
            null,
            null,
            false
        )
    )

    expect(result.borders).toBe('untill 10/10/2010')
})

test('[print] should return from & untill date if current date less', async () => {
    const service = createService(new Date(2010, 9, 9))

    const result = service.print(
        new PlannedRecurrence(
            '0',
            '',
            new Date(2010, 9, 10),
            new Date(2010, 9, 10),
            null,
            null,
            null,
            false
        )
    )

    expect(result.borders).toBe('from 10/10/2010 untill 10/10/2010')
})
