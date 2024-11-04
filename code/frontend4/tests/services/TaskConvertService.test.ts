import { expect, test } from 'vitest'

import { TaskConvertService } from '../../src/edit-task/services/TaskConvertService'
import { DateService } from '../../src/common/services/DateService'
import { TaskTypeEnum } from '../../src/tasks/models/TaskTypeEnum'

function dt(year: number, month: number, date: number): number {
    return new Date(year, month, date).valueOf()
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
test('[convertStringToModel] next monday task through exclamation mark', () => {
    const service = createService(new Date(2019, 6, 28))
    const result = service.convertStringToModel('!1 Test')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2019, 6, 29))
    expect(result.time).toBeNull()
})

// #16
test('[convertStringToModel] next wednesday task through exclamation mark', () => {
    const service = createService(new Date(2019, 6, 28))
    const result = service.convertStringToModel('!3 Test')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2019, 6, 31))
    expect(result.time).toBeNull()
})

// #17
test('[convertStringToModel] next friday next month task through exclamation mark', () => {
    const service = createService(new Date(2019, 6, 28))
    const result = service.convertStringToModel('!5 Test')

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date?.valueOf()).toBe(dt(2019, 7, 2))
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

test('[convertModelToString] no date', () => {
    const service = createService()
    const result = service.convertModelToString({
        title: 'Test!',
        type: TaskTypeEnum.Simple,
        date: null,
        time: null,
        isProbable: false,
    })
    expect(result).toBe('Test!')
})

test('[convertModelToString] date & no time', () => {
    const service = createService()
    const result = service.convertModelToString({
        title: 'Test!',
        type: TaskTypeEnum.Simple,
        date: new Date(2019, 11, 11),
        time: null,
        isProbable: false,
    })
    expect(result).toBe('1211 Test!')
})

test('[convertModelToString] date & time', () => {
    const service = createService()
    const result = service.convertModelToString({
        title: 'Test!',
        type: TaskTypeEnum.Simple,
        date: new Date(2019, 11, 11),
        time: 1439,
        isProbable: false,
    })
    expect(result).toBe('1211 2359 Test!')
})

test('[convertModelToString] date & time less ten', () => {
    const service = createService()
    const result = service.convertModelToString({
        title: 'Test!',
        type: TaskTypeEnum.Simple,
        date: new Date(2019, 0, 1),
        time: 61,
        isProbable: false,
    })
    expect(result).toBe('0101 0101 Test!')
})

test('[convertModelToString] is probable', () => {
    const service = createService()
    const result = service.convertModelToString({
        title: 'Test!',
        type: TaskTypeEnum.Simple,
        date: null,
        time: null,
        isProbable: true,
    })
    expect(result).toBe('Test! ?')
})

test('[convertModelToString] additional', () => {
    const service = createService()
    const result = service.convertModelToString({
        title: 'Test!',
        type: TaskTypeEnum.Additional,
        date: new Date(2019, 0, 1),
        time: null,
        isProbable: false,
    })
    expect(result).toBe('0101 Test! !')
})

test('[convertModelToString] additional and probable', () => {
    const service = createService()
    const result = service.convertModelToString({
        title: 'Test!',
        type: TaskTypeEnum.Additional,
        date: new Date(2019, 0, 1),
        time: null,
        isProbable: true,
    })
    expect(result).toBe('0101 Test! !?')
})

test('[convertModelToString] date with year', () => {
    const service = createService()
    const result = service.convertModelToString({
        title: 'Test!',
        type: TaskTypeEnum.Simple,
        date: new Date(2016, 11, 11),
        time: null,
        isProbable: false,
    })
    expect(result).toBe('20161211 Test!')
})

test('[convertModelToString] routine', () => {
    const service = createService()
    const result = service.convertModelToString({
        title: 'Test!',
        type: TaskTypeEnum.Routine,
        date: new Date(2019, 0, 1),
        time: null,
        isProbable: false,
    })
    expect(result).toBe('0101 Test! *')
})

test('[convertModelToString] routine and probable', () => {
    const service = createService()
    console.log(service, 'ewrwereeee')
    const result = service.convertModelToString({
        title: 'Test!',
        type: TaskTypeEnum.Routine,
        date: new Date(2019, 0, 1),
        time: null,
        isProbable: true,
    })
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
