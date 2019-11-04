import { RecurrenceService, DateService } from '../../di'
import { PlannedRecurrence, RecurrenceWeekdayEnum } from '../../models'

test('[print] should return task title', async() => {
    const service = new RecurrenceService(new DateService())

    const result = service.print(new PlannedRecurrence(0, 'Some task text', new Date(), null, null, null, null))

    expect(result).toContain('Some task text')
})

test('[print] should return weekly', async() => {
    const service = new RecurrenceService(new DateService())

    const result = service.print(new PlannedRecurrence(0, '', new Date(), null, null, null, RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Saturday | RecurrenceWeekdayEnum.Sunday))

    expect(result).toContain('weekly on Monday, Saturday, Sunday')
})

test('[print] should return every weekday', async() => {
    const service = new RecurrenceService(new DateService())

    const result = service.print(new PlannedRecurrence(0, '', new Date(), null, null, null, RecurrenceWeekdayEnum.Monday | RecurrenceWeekdayEnum.Tuesday | RecurrenceWeekdayEnum.Wednesday | RecurrenceWeekdayEnum.Thursday | RecurrenceWeekdayEnum.Friday))

    expect(result).toContain('every weekday (Monday to Friday)')
})

test('[print] should return monthly', async() => {
    const service = new RecurrenceService(new DateService())

    const result = service.print(new PlannedRecurrence(0, '', new Date(), null, null, '1,3,10', null))

    expect(result).toContain('monthly on 1, 3, 10 dates')
})

test('[print] should return monthly (one date)', async() => {
    const service = new RecurrenceService(new DateService())

    const result = service.print(new PlannedRecurrence(0, '', new Date(), null, null, '3', null))

    expect(result).toContain('monthly on 3 date')
})

test('[print] should return daily', async() => {
    const service = new RecurrenceService(new DateService())

    const result = service.print(new PlannedRecurrence(0, '', new Date(), null, 1, null, null))

    expect(result).toContain('daily')
})

test('[print] should return every 2nd day', async() => {
    const service = new RecurrenceService(new DateService())

    const result = service.print(new PlannedRecurrence(0, '', new Date(), null, 2, null, null))

    expect(result).toContain('every 2nd day')
})

test('[print] should return every 3rd day', async() => {
    const service = new RecurrenceService(new DateService())

    const result = service.print(new PlannedRecurrence(0, '', new Date(), null, 3, null, null))

    expect(result).toContain('every 3rd day')
})

test('[print] should return every Nth day', async() => {
    const service = new RecurrenceService(new DateService())

    const result = service.print(new PlannedRecurrence(0, '', new Date(), null, 10, null, null))

    expect(result).toContain('every 10th day')
})

test('[print] should return combo nth & weekday', async() => {
    const service = new RecurrenceService(new DateService())

    const result = service.print(new PlannedRecurrence(0, '', new Date(), null, 1, null, RecurrenceWeekdayEnum.Monday))

    expect(result).toContain('every 1st day on Monday')
})

test('[print] should return combo nth & month day', async() => {
    const service = new RecurrenceService(new DateService())

    const result = service.print(new PlannedRecurrence(0, '', new Date(), null, 2, '19', null))

    expect(result).toContain('every 2nd day on 19 date')
})

test('[print] should return combo nth & weekday & month day', async() => {
    const service = new RecurrenceService(new DateService())

    const result = service.print(new PlannedRecurrence(0, '', new Date(), null, 3, '21,28', RecurrenceWeekdayEnum.Friday | RecurrenceWeekdayEnum.Saturday))

    expect(result).toContain('every 3rd day on Friday, Saturday and 21, 28 dates')
})

test('[print] should return combo weekday & month day', async() => {
    const service = new RecurrenceService(new DateService())

    const result = service.print(new PlannedRecurrence(0, '', new Date(), null, null, '13', RecurrenceWeekdayEnum.Friday))

    expect(result).toContain('on Friday and 13 date')
})

test('[print] should return from date', async() => {
    const service = new RecurrenceService(new DateService())

    const result = service.print(new PlannedRecurrence(0, '', new Date(2010, 9, 10), null, null, null, null))

    expect(result).toContain('from 10/10/2010')
})

test('[print] should return untill date', async() => {
    const service = new RecurrenceService(new DateService())

    const result = service.print(new PlannedRecurrence(0, '', new Date(), new Date(2010, 9, 10), null, null, null))

    expect(result).toContain('untill 10/10/2010')
})
