import { expect, test } from 'vitest'

import { TaskConvertService } from '../../src/edit-task/services/TaskConvertService'
import { DateService } from '../../src/common/services/DateService'
import { TaskTypeEnum } from '../../src/tasks/models/TaskTypeEnum'
import { TaskModel } from '../../src/tasks/models/TaskModel'

function dt(year: number, month: number, date: number): number {
    return new Date(year, month, date).valueOf()
}

function task(partial: Partial<TaskModel>): TaskModel {
    return {
        uid: 'uid',
        title: '',
        date: null,
        order: 0,
        completed: false,
        deleted: false,
        type: TaskTypeEnum.Simple,
        isProbable: false,
        version: 0,
        time: null,
        ...partial,
    }
}

function createService(date?: Date): TaskConvertService {
    if (date === undefined) {
        date = new Date(2019, 0, 1)
    }

    const dateServiceMock: DateService = {
        today: () => date,
    } as unknown as DateService

    return new TaskConvertService(dateServiceMock)
}

// [convertStringToModel] tests should be synced with BE TaskParserService.ParseTask tests
// #1
test('[convertStringToModel] no date and time', () => {
    const service = createService()
    const result = service.convertStringToModel('Test!')

    expect(result.title).toBe('Test!')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #2
test('[convertStringToModel] date and no time', () => {
    const service = createService()
    const result = service.convertStringToModel('1231 Test!')

    expect(result.title).toBe('Test!')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2019, 11, 31))
    expect(result.time).toBeNull()
})

// #3
test('[convertStringToModel] date and no time 2 - not working w/o space', () => {
    const service = createService()
    const result = service.convertStringToModel('0101Test!!!')

    expect(result.title).toBe('0101Test!!!')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #4
test('[convertStringToModel] date and time', () => {
    const service = createService()
    const result = service.convertStringToModel('1231 2359 Test!')

    expect(result.title).toBe('Test!')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2019, 11, 31))
    expect(result.time).toBe(1439)
})

// #5
test('[convertStringToModel] date and time 2 - not working w/o space', () => {
    const service = createService()
    const result = service.convertStringToModel('0101 0101Test!!!')

    expect(result.title).toBe('0101Test!!!')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2019, 0, 1))
    expect(result.time).toBeNull()
})

// #6
test('[convertStringToModel] date and no time with year', () => {
    const service = createService()
    const result = service.convertStringToModel('20170101 Test')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2017, 0, 1))
    expect(result.time).toBeNull()
})

// #7
test('[convertStringToModel] is probable', () => {
    const service = createService()
    const result = service.convertStringToModel('Test! ?')

    expect(result.title).toBe('Test!')
    expect(result.isProbable).toBe(true)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #8
test('[convertStringToModel] additional with date', () => {
    const service = createService()
    const result = service.convertStringToModel('0220 Test !')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Additional)
    expect(result.date?.valueOf()).toBe(dt(2019, 1, 20))
    expect(result.time).toBeNull()
})

// #9
test('[convertStringToModel] additional with date and time', () => {
    const service = createService()
    const result = service.convertStringToModel('20150220 2359 Test !')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Additional)
    expect(result.date?.valueOf()).toBe(dt(2015, 1, 20))
    expect(result.time).toBe(1439)
})

// #10
test('[convertStringToModel] additional and probable', () => {
    const service = createService()
    const result = service.convertStringToModel('Test !?')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Additional)
    expect(result.isProbable).toBe(true)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #10.1
test('[convertStringToModel] probable and additional', () => {
    const service = createService()
    const result = service.convertStringToModel('Test ?!')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Additional)
    expect(result.isProbable).toBe(true)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #11
test('[convertStringToModel] today task through exclamation mark', () => {
    const service = createService()
    const result = service.convertStringToModel('! Test')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2019, 0, 1))
    expect(result.time).toBeNull()
})

// #12
test('[convertStringToModel] tomorrow task through exclamation mark', () => {
    const service = createService()
    const result = service.convertStringToModel('!! Test')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2019, 0, 2))
    expect(result.time).toBeNull()
})

// #13
test('[convertStringToModel] day after after tomorrow task through exclamation mark', () => {
    const service = createService()
    const result = service.convertStringToModel('!!!! Test')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2019, 0, 4))
    expect(result.time).toBeNull()
})

// #14
test('[convertStringToModel] day after tomorrow next month task through exclamation mark', () => {
    const service = createService(new Date(2019, 0, 31))
    const result = service.convertStringToModel('!!! Test')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2019, 1, 2))
    expect(result.time).toBeNull()
})

// #15
test('[convertStringToModel] this week monday task through exclamation mark', () => {
    const service = createService(new Date(2019, 6, 31))
    const result = service.convertStringToModel('!1 Test')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2019, 6, 29))
    expect(result.time).toBeNull()
})

// #15.1
test('[convertStringToModel] next monday task through exclamation mark', () => {
    const service = createService(new Date(2019, 6, 28))
    const result = service.convertStringToModel('!!1 Test')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2019, 6, 29))
    expect(result.time).toBeNull()
})

// #16
test('[convertStringToModel] next wednesday task through exclamation mark', () => {
    const service = createService(new Date(2019, 6, 28))
    const result = service.convertStringToModel('!!3 Test')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2019, 6, 31))
    expect(result.time).toBeNull()
})

// #17
test('[convertStringToModel] next friday next month task through exclamation mark', () => {
    const service = createService(new Date(2019, 6, 28))
    const result = service.convertStringToModel('!!5 Test')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2019, 7, 2))
    expect(result.time).toBeNull()
})

// #17.1
test('[convertStringToModel] week after next monday task through exclamation mark', () => {
    const service = createService(new Date(2019, 6, 28))
    const result = service.convertStringToModel('!!!1 Test')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2019, 7, 5))
    expect(result.time).toBeNull()
})

// #18
test('[convertStringToModel] !11 is not week shift pattern', () => {
    const service = createService()
    const result = service.convertStringToModel('!11 Test')

    expect(result.title).toBe('!11 Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #19
test('[convertStringToModel] date with exclamation', () => {
    const service = createService()
    const result = service.convertStringToModel('1231! Test')

    expect(result.title).toBe('1231! Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #20
test('[convertStringToModel] additional with date', () => {
    const service = createService()
    const result = service.convertStringToModel('0220 Test *')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Routine)
    expect(result.date?.valueOf()).toBe(dt(2019, 1, 20))
    expect(result.time).toBeNull()
})

// #21
test('[convertStringToModel] additional with date and time', () => {
    const service = createService()
    const result = service.convertStringToModel('20150220 2359 Test *')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Routine)
    expect(result.date?.valueOf()).toBe(dt(2015, 1, 20))
    expect(result.time).toBe(1439)
})

// #22
test('[convertStringToModel] additional and probable', () => {
    const service = createService()
    const result = service.convertStringToModel('Test *?')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Routine)
    expect(result.isProbable).toBe(true)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #22.1
test('[convertStringToModel] probable and additional', () => {
    const service = createService()
    const result = service.convertStringToModel('Test ?*')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Routine)
    expect(result.isProbable).toBe(true)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #23
test('[convertStringToModel] routine and additional', () => {
    const service = createService()
    const result = service.convertStringToModel('Test !*')

    expect(result.title).toBe('Test !*')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.isProbable).toBe(false)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #24
test('[convertStringToModel] weekly task', () => {
    const service = createService()
    const result = service.convertStringToModel('Test %')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Weekly)
    expect(result.isProbable).toBe(false)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #25
test('[convertStringToModel] weekly and probable', () => {
    const service = createService()
    const result = service.convertStringToModel('Test %?')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Weekly)
    expect(result.isProbable).toBe(true)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #25.1
test('[convertStringToModel] probable and weekly', () => {
    const service = createService()
    const result = service.convertStringToModel('Test ?%')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Weekly)
    expect(result.isProbable).toBe(true)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #26
test('[convertStringToModel] weekly duplicate cancels', () => {
    const service = createService()
    const result = service.convertStringToModel('Test %%')

    expect(result.title).toBe('Test %%')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.isProbable).toBe(false)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #27
test('[convertStringToModel] weekly and routine conflict', () => {
    const service = createService()
    const result = service.convertStringToModel('Test %*')

    expect(result.title).toBe('Test %*')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.isProbable).toBe(false)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #28
test('[convertStringToModel] probable weekly probable conflict', () => {
    const service = createService()
    const result = service.convertStringToModel('Test ?%?')

    expect(result.title).toBe('Test ?%?')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.isProbable).toBe(false)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// [Date range] parsing + expansion tests. Range day-count and validity live in
// TaskRangeService.test.ts. Together they mirror BE TaskParserService.ParseTasks range
// tests (#29-#39), which combine parse + validate + expand in one method.

// #29
test('[convertStringToModel] date range without year', () => {
    const service = createService()
    const result = service.convertStringToModel('0909-0913 venice')

    expect(result.title).toBe('venice')
    expect(result.date?.valueOf()).toBe(dt(2019, 8, 9))
    expect(result.dateTo?.valueOf()).toBe(dt(2019, 8, 13))
    expect(result.time).toBeNull()
})

// #30
test('[createTasksFromModel] date range with time and type applies to every task', () => {
    const service = createService()
    const tasks = service.createTasksFromModel(service.convertStringToModel('0909-0913 1012 venice !'))

    expect(tasks.length).toBe(5)
    expect(tasks[0].date).toBe(dt(2019, 8, 9))
    expect(tasks[4].date).toBe(dt(2019, 8, 13))
    expect(tasks.every(t => t.time === 612)).toBe(true)
    expect(tasks.every(t => t.type === TaskTypeEnum.Additional)).toBe(true)
    expect(tasks.every(t => t.title === 'venice')).toBe(true)
})

// #31
test('[convertStringToModel] date range with year on both endpoints', () => {
    const service = createService()
    const result = service.convertStringToModel('20270909-20270915 venice')

    expect(result.title).toBe('venice')
    expect(result.date?.valueOf()).toBe(dt(2027, 8, 9))
    expect(result.dateTo?.valueOf()).toBe(dt(2027, 8, 15))
})

// #32
test('[convertStringToModel] date range with mixed year endpoints', () => {
    const service = createService()
    const result = service.convertStringToModel('0909-20190915 venice')

    expect(result.title).toBe('venice')
    expect(result.date?.valueOf()).toBe(dt(2019, 8, 9))
    expect(result.dateTo?.valueOf()).toBe(dt(2019, 8, 15))
})

// #33
test('[convertStringToModel] date range not parsed without space', () => {
    const service = createService()
    const result = service.convertStringToModel('0909-0913venice')

    expect(result.title).toBe('0909-0913venice')
    expect(result.date).toBeNull()
    expect(result.dateTo).toBeNull()
})

// #35 (expansion side; day-count/validity asserted in TaskRangeService.test.ts)
test('[createTasksFromModel] range at the limit creates one task per inclusive day', () => {
    const service = createService()
    const tasks = service.createTasksFromModel(service.convertStringToModel('0901-1001 venice'))

    expect(tasks.length).toBe(31)
    expect(tasks[0].date).toBe(dt(2019, 8, 1))
    expect(tasks[30].date).toBe(dt(2019, 9, 1))
})

// #39 (expansion side; day-count/validity asserted in TaskRangeService.test.ts)
test('[createTasksFromModel] minimum valid range creates two tasks', () => {
    const service = createService()
    const tasks = service.createTasksFromModel(service.convertStringToModel('0910-0911 venice'))

    expect(tasks.length).toBe(2)
    expect(tasks[0].date).toBe(dt(2019, 8, 10))
    expect(tasks[1].date).toBe(dt(2019, 8, 11))
})

// [FE-only] convertStringToModel parses time and type alongside a range (model-level check)
test('[convertStringToModel] date range with time and type', () => {
    const service = createService()
    const result = service.convertStringToModel('0909-0913 1012 venice !')

    expect(result.title).toBe('venice')
    expect(result.type).toBe(TaskTypeEnum.Additional)
    expect(result.date?.valueOf()).toBe(dt(2019, 8, 9))
    expect(result.dateTo?.valueOf()).toBe(dt(2019, 8, 13))
    expect(result.time).toBe(612)
})

// [FE-only] createTasksFromModel without a range yields a single task (dateTo === null branch)
test('[createTasksFromModel] no range creates one task', () => {
    const service = createService()
    const tasks = service.createTasksFromModel(service.convertStringToModel('0909 venice'))

    expect(tasks.length).toBe(1)
    expect(tasks[0].date).toBe(dt(2019, 8, 9))
})

test('[convertTaskToString] no date', () => {
    const service = createService()
    const result = service.convertTaskToString(task({ title: 'Test!' }))
    expect(result).toBe('Test!')
})

test('[convertTaskToString] date & no time', () => {
    const service = createService()
    const result = service.convertTaskToString(task({ title: 'Test!', date: dt(2019, 11, 11) }))
    expect(result).toBe('1211 Test!')
})

test('[convertTaskToString] date & time', () => {
    const service = createService()
    const result = service.convertTaskToString(task({ title: 'Test!', date: dt(2019, 11, 11), time: 1439 }))
    expect(result).toBe('1211 2359 Test!')
})

test('[convertTaskToString] date & time less ten', () => {
    const service = createService()
    const result = service.convertTaskToString(task({ title: 'Test!', date: dt(2019, 0, 1), time: 61 }))
    expect(result).toBe('0101 0101 Test!')
})

test('[convertTaskToString] is probable', () => {
    const service = createService()
    const result = service.convertTaskToString(task({ title: 'Test!', isProbable: true }))
    expect(result).toBe('Test! ?')
})

test('[convertTaskToString] additional', () => {
    const service = createService()
    const result = service.convertTaskToString(
        task({ title: 'Test!', type: TaskTypeEnum.Additional, date: dt(2019, 0, 1) }),
    )
    expect(result).toBe('0101 Test! !')
})

test('[convertTaskToString] additional and probable', () => {
    const service = createService()
    const result = service.convertTaskToString(
        task({ title: 'Test!', type: TaskTypeEnum.Additional, date: dt(2019, 0, 1), isProbable: true }),
    )
    expect(result).toBe('0101 Test! !?')
})

test('[convertTaskToString] date with year', () => {
    const service = createService()
    const result = service.convertTaskToString(task({ title: 'Test!', date: dt(2016, 11, 11) }))
    expect(result).toBe('20161211 Test!')
})

test('[convertTaskToString] routine', () => {
    const service = createService()
    const result = service.convertTaskToString(
        task({ title: 'Test!', type: TaskTypeEnum.Routine, date: dt(2019, 0, 1) }),
    )
    expect(result).toBe('0101 Test! *')
})

test('[convertTaskToString] routine and probable', () => {
    const service = createService()
    const result = service.convertTaskToString(
        task({ title: 'Test!', type: TaskTypeEnum.Routine, date: dt(2019, 0, 1), isProbable: true }),
    )
    expect(result).toBe('0101 Test! *?')
})

test('[convertDateToString] current year', () => {
    const service = createService()
    const result = service.convertDateToString(new Date(2019, 3, 9))
    expect(result).toBe('0409')
})

test('[convertDateToString] another year', () => {
    const service = createService()
    const result = service.convertDateToString(new Date(2025, 3, 9))
    expect(result).toBe('20250409')
})

test('[toDateLabel] current year', () => {
    const service = createService()
    const result = service.toDateLabel(new Date(2019, 3, 9))
    expect(result).toBe('04/09 Tue')
})

test('[toDateLabel] another year', () => {
    const service = createService()
    const result = service.toDateLabel(new Date(2025, 3, 9))
    expect(result).toBe('2025/04/09 Wed')
})
