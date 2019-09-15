import { TaskConverter } from '../../di'
import { TaskModel, TaskTypeEnum } from '../../models'

const def: Date = new Date(2019, 0, 1, 0, 0)
function dt(year: number, month: number, date: number): number {
    return new Date(Date.UTC(year, month, date)).getTime()
}

// [convertStringToModel] tests should be synced with BE TaskParserService.ParseTask tests
// #1
test('[convertStringToModel] no date and time', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('Test!', def)

    expect(result.title).toBe('Test!')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #2
test('[convertStringToModel] date and no time', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('1231 Test!', def)

    expect(result.title).toBe('Test!')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date!.getTime()).toBe(dt(2019, 11, 31))
    expect(result.time).toBeNull()
})

// #3
test('[convertStringToModel] date and no time 2 - not working w/o space', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('0101Test!!!', def)

    expect(result.title).toBe('0101Test!!!')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #4
test('[convertStringToModel] date and time', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('1231 2359 Test!', def)

    expect(result.title).toBe('Test!')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date!.getTime()).toBe(dt(2019, 11, 31))
    expect(result.time).toBe(1439)
})

// #5
test('[convertStringToModel] date and time 2 - not working w/o space', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('0101 0101Test!!!', def)

    expect(result.title).toBe('0101Test!!!')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date!.getTime()).toBe(dt(2019, 0, 1))
    expect(result.time).toBeNull()
})

// #6
test('[convertStringToModel] date and no time with year', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('20170101 Test', def)

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date!.getTime()).toBe(dt(2017, 0, 1))
    expect(result.time).toBeNull()
})

// #7
test('[convertStringToModel] is probable', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('Test! ?', def)

    expect(result.title).toBe('Test!')
    expect(result.isProbable).toBe(true)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

// #8
test('[convertStringToModel] additional with short date', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('0220! Test', def)

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Additional)
    expect(result.date!.getTime()).toBe(dt(2019, 1, 20))
    expect(result.time).toBeNull()
})

// #9
test('[convertStringToModel] additional with long date', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('20150220! Test', def)

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Additional)
    expect(result.date!.getTime()).toBe(dt(2015, 1, 20))
    expect(result.time).toBeNull()
})

// #10
test('[convertStringToModel] ignore time if additional', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('20150606! 2359 Test', def)

    expect(result.title).toBe('2359 Test')
    expect(result.type).toBe(TaskTypeEnum.Additional)
    expect(result.date!.getTime()).toBe(dt(2015, 5, 6))
    expect(result.time).toBeNull()
})

// #11
test('[convertStringToModel] today task through exclamation mark', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('! Test', def)

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date!.getTime()).toBe(dt(2019, 0, 1))
    expect(result.time).toBeNull()
})

// #12
test('[convertStringToModel] tomorrow task through exclamation mark', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('!! Test', def)

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date!.getTime()).toBe(dt(2019, 0, 2))
    expect(result.time).toBeNull()
})

// #13
test('[convertStringToModel] day after after tomorrow task through exclamation mark', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('!!!! Test', def)

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date!.getTime()).toBe(dt(2019, 0, 4))
    expect(result.time).toBeNull()
})

// #14
test('[convertStringToModel] day after tomorrow next month task through exclamation mark', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('!!! Test', new Date(2019, 0, 31))

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date!.getTime()).toBe(dt(2019, 1, 2))
    expect(result.time).toBeNull()
})

// #15
test('[convertStringToModel] next monday task through exclamation mark', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('!1 Test', new Date(2019, 6, 28))

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date!.getTime()).toBe(dt(2019, 6, 29))
    expect(result.time).toBeNull()
})

// #16
test('[convertStringToModel] next wednesday task through exclamation mark', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('!3 Test', new Date(2019, 6, 28))

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date!.getTime()).toBe(dt(2019, 6, 31))
    expect(result.time).toBeNull()
})

// #17
test('[convertStringToModel] next friday next month task through exclamation mark', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('!5 Test', new Date(2019, 6, 28))

    expect(result.title).toBe('Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date!.getTime()).toBe(dt(2019, 7, 2))
    expect(result.time).toBeNull()
})

// #18
test('[convertStringToModel] !11 is not week shift pattern', () => {
    const service = new TaskConverter()
    const result = service.convertStringToModel('!11 Test', def)

    expect(result.title).toBe('!11 Test')
    expect(result.type).toBe(TaskTypeEnum.Simple)
    expect(result.date).toBeNull()
    expect(result.time).toBeNull()
})

test('[convertModelToString] no date', () => {
    const service = new TaskConverter()
    const result = service.convertModelToString(new TaskModel('Test!'))
    expect(result).toBe('Test!')
})

test('[convertModelToString] date & no time', () => {
    const service = new TaskConverter()
    const result = service.convertModelToString(new TaskModel('Test!',
        new Date(new Date().getFullYear(), 11, 11)
    ))
    expect(result).toBe('1211 Test!')
})

test('[convertModelToString] date & time', () => {
    const service = new TaskConverter()
    const result = service.convertModelToString(new TaskModel('Test!',
        new Date(new Date().getFullYear(), 11, 11),
        TaskTypeEnum.Simple,
        false,
        1439
    ))
    expect(result).toBe('1211 2359 Test!')
})

test('[convertModelToString] date & time less ten', () => {
    const service = new TaskConverter()
    const result = service.convertModelToString(new TaskModel('Test!',
        new Date(new Date().getFullYear(), 0, 1),
        TaskTypeEnum.Simple,
        false,
        61
    ))
    expect(result).toBe('0101 0101 Test!')
})

test('[convertModelToString] is probable', () => {
    const service = new TaskConverter()
    const result = service.convertModelToString(new TaskModel('Test!',
        null,
        TaskTypeEnum.Simple,
        true
    ))
    expect(result).toBe('Test! ?')
})

test('[convertModelToString] additional', () => {
    const service = new TaskConverter()
    const result = service.convertModelToString(new TaskModel('Test!',
        new Date(new Date().getFullYear(), 0, 1),
        TaskTypeEnum.Additional
    ))
    expect(result).toBe('0101! Test!')
})

test('[convertModelToString] date with year', () => {
    const service = new TaskConverter()
    const result = service.convertModelToString(new TaskModel('Test!',
        new Date(2016, 11, 11)
    ))
    expect(result).toBe('20161211 Test!')
})
