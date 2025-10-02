import { expect, test } from 'vitest'
import { DateService } from '../../src/common/services/DateService'

test('[toDateLabel] positive', () => {
    const service = new DateService()
    expect(service.toDateLabel(new Date(2018, 9, 21))).toBe('10/21/2018 Sun')
})

test('[toTimeLabel] positive', () => {
    const service = new DateService()
    expect(service.toTimeLabel(600)).toBe('10:00')
})

test('[getWeekdayName] positive', () => {
    const service = new DateService()
    expect(service.getWeekdayName(new Date(2018, 9, 21))).toBe('Sun')
})

test('[monday] positive', () => {
    const service = new DateService()
    expect(service.monday(new Date(2018, 9, 17)).valueOf()).toBe(new Date(2018, 9, 15).valueOf())
})
