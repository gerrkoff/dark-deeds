import { expect, test } from 'vitest'
import { DateMaskService } from '../../src/recurrences/services/DateMaskService'
import { DateService } from '../../src/common/services/DateService'

function service(): DateMaskService {
    return new DateMaskService(new DateService())
}

test('[applyMask] masks eight digits into dd/mm/yyyy', () => {
    expect(service().applyMask('08072026', '')).toBe('08/07/2026')
})

test('[applyMask] appends a slash after two digits while typing', () => {
    expect(service().applyMask('08', '0')).toBe('08/')
})

test('[applyMask] appends a slash after four digits while typing', () => {
    expect(service().applyMask('08/07', '08/0')).toBe('08/07/')
})

test('[applyMask] removes exactly one char on backspace', () => {
    expect(service().applyMask('08/07/202', '08/07/2026')).toBe('08/07/202')
})

test('[applyMask] does not re-add an auto slash when backspacing over it', () => {
    expect(service().applyMask('08', '08/')).toBe('08')
})

test('[applyMask] strips non-digit characters', () => {
    expect(service().applyMask('1a', '1')).toBe('1')
})

test('[applyMask] caps the input at eight digits', () => {
    expect(service().applyMask('0807202699', '')).toBe('08/07/2026')
})

test('[isValidDate] accepts a valid date', () => {
    expect(service().isValidDate('08/07/2026')).toBe(true)
})

test('[isValidDate] accepts a leap-year day', () => {
    expect(service().isValidDate('29/02/2024')).toBe(true)
})

test('[isValidDate] rejects an overflowing day', () => {
    expect(service().isValidDate('31/02/2026')).toBe(false)
})

test('[isValidDate] rejects an overflowing month and day', () => {
    expect(service().isValidDate('45/13/2026')).toBe(false)
})

test('[isValidDate] rejects a non-leap-year 29 February', () => {
    expect(service().isValidDate('29/02/2023')).toBe(false)
})

test('[isValidDate] rejects a zero day', () => {
    expect(service().isValidDate('00/01/2026')).toBe(false)
})

test('[isValidDate] rejects a zero month', () => {
    expect(service().isValidDate('01/00/2026')).toBe(false)
})

test('[isValidDate] rejects an empty value', () => {
    expect(service().isValidDate('')).toBe(false)
})

test('[isValidDate] rejects a partial value', () => {
    expect(service().isValidDate('08/07')).toBe(false)
})

test('[toTimestamp] converts a valid value to epoch ms', () => {
    expect(service().toTimestamp('08/07/2026')).toBe(new Date(2026, 6, 8).valueOf())
})

test('[toTimestamp] returns null for an invalid value', () => {
    expect(service().toTimestamp('31/02/2026')).toBeNull()
})

test('[toTimestamp] returns null for an empty value', () => {
    expect(service().toTimestamp('')).toBeNull()
})

test('[fromTimestamp] converts epoch ms to a padded dd/mm/yyyy', () => {
    expect(service().fromTimestamp(new Date(2026, 6, 8).valueOf())).toBe('08/07/2026')
})

test('[fromTimestamp] returns an empty string for null', () => {
    expect(service().fromTimestamp(null)).toBe('')
})

test('[fromTimestamp] returns an empty string for undefined', () => {
    expect(service().fromTimestamp(undefined)).toBe('')
})

test('[fromTimestamp] returns an empty string for zero', () => {
    expect(service().fromTimestamp(0)).toBe('')
})
