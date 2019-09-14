import { TaskConverter } from '../../services'
import { TaskModel, TaskTimeTypeEnum } from '../../models'

const def: Date = new Date(2019, 0, 1, 0, 0)
function dt(year: number, month: number, date: number): number {
    return new Date(Date.UTC(year, month, date)).getTime()
}

// [convertStringToModel] tests should be synced with BE TaskParserService.ParseTask tests
// #1
test('[convertStringToModel] no date and time', () => {
    const result = TaskConverter.convertStringToModel('Test!', def)

    expect(result.title).toBe('Test!')
    expect(result.date).toBeNull()
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

// #2
test('[convertStringToModel] date and no time', () => {
    const result = TaskConverter.convertStringToModel('1231 Test!', def)

    expect(result.title).toBe('Test!')
    expect(result.date!.getTime()).toBe(dt(2019, 11, 31))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
    expect(result.time).toBeNull()
})

// #3
test('[convertStringToModel] date and no time 2 - not working w/o space', () => {
    const result = TaskConverter.convertStringToModel('0101Test!!!', def)

    expect(result.title).toBe('0101Test!!!')
    expect(result.date).toBe(null)
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
    expect(result.time).toBeNull()
})

// #4
test('[convertStringToModel] date and time', () => {
    const result = TaskConverter.convertStringToModel('1231 2359 Test!', def)

    expect(result.title).toBe('Test!')
    expect(result.date!.getTime()).toBe(dt(2019, 11, 31))
    expect(result.time).toBe(1439)
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

// #5
test('[convertStringToModel] date and time 2 - not working w/o space', () => {
    const result = TaskConverter.convertStringToModel('0101 0101Test!!!', def)

    expect(result.title).toBe('0101Test!!!')
    expect(result.date!.getTime()).toBe(dt(2019, 0, 1))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
    expect(result.time).toBeNull()
})

// #6
test('[convertStringToModel] date and no time with year', () => {
    const result = TaskConverter.convertStringToModel('20170101 Test', def)

    expect(result.title).toBe('Test')
    expect(result.date!.getTime()).toBe(dt(2017, 0, 1))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
    expect(result.time).toBeNull()
})

// #7
test('[convertStringToModel] is probable', () => {
    const result = TaskConverter.convertStringToModel('Test! ?', def)

    expect(result.title).toBe('Test!')
    expect(result.isProbable).toBe(true)
    expect(result.time).toBeNull()
})

// #8
test('[convertStringToModel] all day long with short date', () => {
    const result = TaskConverter.convertStringToModel('0220! Test', def)

    expect(result.title).toBe('Test')
    expect(result.date!.getTime()).toBe(dt(2019, 1, 20))
    expect(result.timeType).toBe(TaskTimeTypeEnum.AllDayLong)
    expect(result.time).toBeNull()
})

// #9
test('[convertStringToModel] all day long with long date', () => {
    const result = TaskConverter.convertStringToModel('20150220! Test', def)

    expect(result.title).toBe('Test')
    expect(result.date!.getTime()).toBe(dt(2015, 1, 20))
    expect(result.timeType).toBe(TaskTimeTypeEnum.AllDayLong)
    expect(result.time).toBeNull()
})

// #10
test('[convertStringToModel] ignore time if all day long', () => {
    const result = TaskConverter.convertStringToModel('20150606! 2359 Test', def)

    expect(result.title).toBe('2359 Test')
    expect(result.date!.getTime()).toBe(dt(2015, 5, 6))
    expect(result.timeType).toBe(TaskTimeTypeEnum.AllDayLong)
    expect(result.time).toBeNull()
})

// #11
test('[convertStringToModel] today task through exclamation mark', () => {
    const result = TaskConverter.convertStringToModel('! Test', def)

    expect(result.title).toBe('Test')
    expect(result.date!.getTime()).toBe(dt(2019, 0, 1))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
    expect(result.time).toBeNull()
})

// #12
test('[convertStringToModel] tomorrow task through exclamation mark', () => {
    const result = TaskConverter.convertStringToModel('!! Test', def)

    expect(result.title).toBe('Test')
    expect(result.date!.getTime()).toBe(dt(2019, 0, 2))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
    expect(result.time).toBeNull()
})

// #13
test('[convertStringToModel] day after after tomorrow task through exclamation mark', () => {
    const result = TaskConverter.convertStringToModel('!!!! Test', def)

    expect(result.title).toBe('Test')
    expect(result.date!.getTime()).toBe(dt(2019, 0, 4))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
    expect(result.time).toBeNull()
})

// #14
test('[convertStringToModel] day after tomorrow next month task through exclamation mark', () => {
    const result = TaskConverter.convertStringToModel('!!! Test', new Date(2019, 0, 31))

    expect(result.title).toBe('Test')
    expect(result.date!.getTime()).toBe(dt(2019, 1, 2))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
    expect(result.time).toBeNull()
})

// #15
test('[convertStringToModel] next monday task through exclamation mark', () => {
    const result = TaskConverter.convertStringToModel('!1 Test', new Date(2019, 6, 28))

    expect(result.title).toBe('Test')
    expect(result.date!.getTime()).toBe(dt(2019, 6, 29))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
    expect(result.time).toBeNull()
})

// #16
test('[convertStringToModel] next wednesday task through exclamation mark', () => {
    const result = TaskConverter.convertStringToModel('!3 Test', new Date(2019, 6, 28))

    expect(result.title).toBe('Test')
    expect(result.date!.getTime()).toBe(dt(2019, 6, 31))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
    expect(result.time).toBeNull()
})

// #17
test('[convertStringToModel] next friday next month task through exclamation mark', () => {
    const result = TaskConverter.convertStringToModel('!5 Test', new Date(2019, 6, 28))

    expect(result.title).toBe('Test')
    expect(result.date!.getTime()).toBe(dt(2019, 7, 2))
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
    expect(result.time).toBeNull()
})

// #18
test('[convertStringToModel] !11 is not week shift pattern', () => {
    const result = TaskConverter.convertStringToModel('!11 Test', def)

    expect(result.title).toBe('!11 Test')
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

test('[convertModelToString] no date', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!'))
    expect(result).toBe('Test!')
})

test('[convertModelToString] date & no time', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!',
        new Date(new Date().getFullYear(), 11, 11)
    ))
    expect(result).toBe('1211 Test!')
})

test('[convertModelToString] date & time', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!',
        new Date(new Date().getFullYear(), 11, 11),
        TaskTimeTypeEnum.NoTime,
        false,
        1439
    ))
    expect(result).toBe('1211 2359 Test!')
})

test('[convertModelToString] date & time less ten', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!',
        new Date(new Date().getFullYear(), 0, 1),
        TaskTimeTypeEnum.NoTime,
        false,
        61
    ))
    expect(result).toBe('0101 0101 Test!')
})

test('[convertModelToString] is probable', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!',
        null,
        TaskTimeTypeEnum.NoTime,
        true
    ))
    expect(result).toBe('Test! ?')
})

test('[convertModelToString] all day long', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!',
        new Date(new Date().getFullYear(), 0, 1),
        TaskTimeTypeEnum.AllDayLong
    ))
    expect(result).toBe('0101! Test!')
})

test('[convertModelToString] date with year', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!',
        new Date(2016, 11, 11)
    ))
    expect(result).toBe('20161211 Test!')
})
