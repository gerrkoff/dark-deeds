import { expect, test } from 'vitest'

import { TaskConvertService } from '../../src/edit-task/services/TaskConvertService'
import { MAX_RANGE_DAYS, MIN_RANGE_DAYS, TaskRangeService } from '../../src/edit-task/services/TaskRangeService'
import { DateService } from '../../src/common/services/DateService'

// Day-count + validity bounds for date ranges. The user-facing labels live in EditTaskModal.
// These mirror the invalid/valid behaviour of BE TaskParserService.ParseTasks (#34-#39),
// which throws on out-of-bounds ranges; on FE a count outside [MIN, MAX] is what marks a
// range invalid (the parsing/expansion side is covered in TaskConvertService.test.ts).

function parse(text: string) {
    const dateServiceMock = { today: () => new Date(2019, 0, 1) } as unknown as DateService
    return new TaskConvertService(dateServiceMock).convertStringToModel(text)
}

// #34
test('[getRangeDayCount] single-day range is below the minimum', () => {
    const service = new TaskRangeService()
    const dayCount = service.getRangeDayCount(parse('0909-0909 venice'))

    expect(dayCount).toBe(1)
    expect(dayCount !== null && dayCount < MIN_RANGE_DAYS).toBe(true)
})

// #35
test('[getRangeDayCount] range exactly at the limit', () => {
    const service = new TaskRangeService()

    expect(service.getRangeDayCount(parse('0901-1001 venice'))).toBe(MAX_RANGE_DAYS)
})

// #36
test('[getRangeDayCount] range exceeding the limit', () => {
    const service = new TaskRangeService()
    const dayCount = service.getRangeDayCount(parse('0901-1002 venice'))

    expect(dayCount).toBe(32)
    expect(dayCount !== null && dayCount > MAX_RANGE_DAYS).toBe(true)
})

// #37
test('[getRangeDayCount] multi-year range exceeds the limit', () => {
    const service = new TaskRangeService()
    const dayCount = service.getRangeDayCount(parse('20270909-20280913 venice'))

    expect(dayCount !== null && dayCount > MAX_RANGE_DAYS).toBe(true)
})

// #38
test('[getRangeDayCount] reversed range is below the minimum', () => {
    const service = new TaskRangeService()
    const dayCount = service.getRangeDayCount(parse('0913-0909 venice'))

    expect(dayCount !== null && dayCount < MIN_RANGE_DAYS).toBe(true)
})

// #39
test('[getRangeDayCount] minimum valid range', () => {
    const service = new TaskRangeService()

    expect(service.getRangeDayCount(parse('0910-0911 venice'))).toBe(MIN_RANGE_DAYS)
})

test('[getRangeDayCount] returns null without a range', () => {
    const service = new TaskRangeService()

    expect(service.getRangeDayCount(parse('0909 venice'))).toBeNull()
})
