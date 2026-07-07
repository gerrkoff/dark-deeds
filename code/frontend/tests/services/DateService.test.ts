import { expect, test } from 'vitest'
import { DateService } from '../../src/common/services/DateService'

test('[toDateLabel] positive', () => {
    const service = new DateService()
    expect(service.toDateLabel(new Date(2018, 9, 21))).toBe('10/21/2018 Sun')
})

test('[toShortDate] zero-pads single-digit day and month', () => {
    const service = new DateService()
    expect(service.toShortDate(new Date(2026, 6, 8))).toBe('08/07/2026')
})

test('[toShortDate] preserves two-digit day and month', () => {
    const service = new DateService()
    expect(service.toShortDate(new Date(2026, 10, 21))).toBe('21/11/2026')
})

test('[toShortDate] renders the full four-digit year', () => {
    const service = new DateService()
    expect(service.toShortDate(new Date(2026, 0, 1))).toBe('01/01/2026')
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
