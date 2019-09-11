import { TaskConverter } from '../../services'
import { TaskModel, TaskTimeTypeEnum } from '../../models'

const def: Date = new Date(2019, 0, 1, 0, 0)
function dt(year: number, month: number, date: number, hour: number, minute: number): number {
    return new Date(Date.UTC(year, month, date, hour, minute)).getTime()
}

// [convertStringToModel] tests should be synced with BE TaskParserService.ParseTask tests
// #1
test('[convertStringToModel] no date and time', () => {
    const result = TaskConverter.convertStringToModel('Test!', def)

    expect(result.title).toBe('Test!')
    expect(result.dateTime).toBe(null)
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

// #2
test('[convertStringToModel] date and no time', () => {
    const result = TaskConverter.convertStringToModel('1231 Test!', def)

    expect(result.title).toBe('Test!')
    expect(result.dateTime!.getTime()).toBe(dt(2019, 11, 31, 0, 0))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

// #3
test('[convertStringToModel] date and no time 2 - not working w/o space', () => {
    const result = TaskConverter.convertStringToModel('0101Test!!!', def)

    expect(result.title).toBe('0101Test!!!')
    expect(result.dateTime).toBe(null)
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

// #4
test('[convertStringToModel] date and time', () => {
    const result = TaskConverter.convertStringToModel('1231 2359 Test!', def)

    expect(result.title).toBe('Test!')
    expect(result.dateTime!.getTime()).toBe(dt(2019, 11, 31, 23, 59))
    expect(result.timeType).toBe(TaskTimeTypeEnum.ConcreteTime)
})

// #5
test('[convertStringToModel] date and time 2 - not working w/o space', () => {
    const result = TaskConverter.convertStringToModel('0101 0101Test!!!', def)

    expect(result.title).toBe('0101Test!!!')
    expect(result.dateTime!.getTime()).toBe(dt(2019, 0, 1, 0, 0))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

// #6
test('[convertStringToModel] date and no time with year', () => {
    const result = TaskConverter.convertStringToModel('20170101 Test', def)

    expect(result.title).toBe('Test')
    expect(result.dateTime!.getTime()).toBe(dt(2017, 0, 1, 0, 0))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

// #7
test('[convertStringToModel] is probable', () => {
    const result = TaskConverter.convertStringToModel('Test! ?', def)

    expect(result.title).toBe('Test!')
    expect(result.isProbable).toBe(true)
})

// #8
test('[convertStringToModel] all day long with short date', () => {
    const result = TaskConverter.convertStringToModel('0220! Test', def)

    expect(result.title).toBe('Test')
    expect(result.dateTime!.getTime()).toBe(dt(2019, 1, 20, 0, 0))
    expect(result.timeType).toBe(TaskTimeTypeEnum.AllDayLong)
})

// #9
test('[convertStringToModel] all day long with long date', () => {
    const result = TaskConverter.convertStringToModel('20150220! Test', def)

    expect(result.title).toBe('Test')
    expect(result.dateTime!.getTime()).toBe(dt(2015, 1, 20, 0, 0))
    expect(result.timeType).toBe(TaskTimeTypeEnum.AllDayLong)
})

// #10
test('[convertStringToModel] ignore time if all day long', () => {
    const result = TaskConverter.convertStringToModel('20150606! 2359 Test', def)

    expect(result.title).toBe('2359 Test')
    expect(result.dateTime!.getTime()).toBe(dt(2015, 5, 6, 0, 0))
    expect(result.timeType).toBe(TaskTimeTypeEnum.AllDayLong)
})

// #11
test('[convertStringToModel] today task through exclamation mark', () => {
    const result = TaskConverter.convertStringToModel('! Test', def)

    expect(result.title).toBe('Test')
    expect(result.dateTime!.getTime()).toBe(dt(2019, 0, 1, 0, 0))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

// #12
test('[convertStringToModel] tomorrow task through exclamation mark', () => {
    const result = TaskConverter.convertStringToModel('!! Test', def)

    expect(result.title).toBe('Test')
    expect(result.dateTime!.getTime()).toBe(dt(2019, 0, 2, 0, 0))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

// #13
test('[convertStringToModel] day after after tomorrow task through exclamation mark', () => {
    const result = TaskConverter.convertStringToModel('!!!! Test', def)

    expect(result.title).toBe('Test')
    expect(result.dateTime!.getTime()).toBe(dt(2019, 0, 4, 0, 0))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

// #14
test('[convertStringToModel] day after tomorrow next month task through exclamation mark', () => {
    const result = TaskConverter.convertStringToModel('!!! Test', new Date(2019, 0, 31))

    expect(result.title).toBe('Test')
    expect(result.dateTime!.getTime()).toBe(dt(2019, 1, 2, 0, 0))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

// #15
test('[convertStringToModel] next monday task through exclamation mark', () => {
    const result = TaskConverter.convertStringToModel('!1 Test', new Date(2019, 6, 28))

    expect(result.title).toBe('Test')
    expect(result.dateTime!.getTime()).toBe(dt(2019, 6, 29, 0, 0))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

// #16
test('[convertStringToModel] next wednesday task through exclamation mark', () => {
    const result = TaskConverter.convertStringToModel('!3 Test', new Date(2019, 6, 28))

    expect(result.title).toBe('Test')
    expect(result.dateTime!.getTime()).toBe(dt(2019, 6, 31, 0, 0))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

// #17
test('[convertStringToModel] next friday next month task through exclamation mark', () => {
    const result = TaskConverter.convertStringToModel('!5 Test', new Date(2019, 6, 28))

    expect(result.title).toBe('Test')
    expect(result.dateTime!.getTime()).toBe(dt(2019, 7, 2, 0, 0))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

// #18
test('[convertStringToModel] !11 is not week shift pattern', () => {
    const result = TaskConverter.convertStringToModel('!11 Test', def)

    expect(result.title).toBe('!11 Test')
})

test('[convertModelToString] no date', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!'))
    expect(result).toBe('Test!')
})

test('[convertModelToString] date & no time', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!', new Date(new Date().getFullYear(), 11, 11)))
    expect(result).toBe('1211 Test!')
})

test('[convertModelToString] date & time', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!', new Date(new Date().getFullYear(), 11, 11, 23, 59), TaskTimeTypeEnum.ConcreteTime))
    expect(result).toBe('1211 2359 Test!')
})

test('[convertModelToString] date & time less ten', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!', new Date(new Date().getFullYear(), 0, 1, 1, 1), TaskTimeTypeEnum.ConcreteTime))
    expect(result).toBe('0101 0101 Test!')
})

test('[convertModelToString] is probable', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!', null, TaskTimeTypeEnum.NoTime, true))
    expect(result).toBe('Test! ?')
})

test('[convertModelToString] all day long', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!', new Date(new Date().getFullYear(), 0, 1, 10, 10), TaskTimeTypeEnum.AllDayLong))
    expect(result).toBe('0101! Test!')
})

test('[convertModelToString] date with year', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!', new Date(2016, 11, 11)))
    expect(result).toBe('20161211 Test!')
})
