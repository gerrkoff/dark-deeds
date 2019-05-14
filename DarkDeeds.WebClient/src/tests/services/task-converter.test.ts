import { TaskConverter } from '../../services'
import { TaskModel, TaskTimeTypeEnum } from '../../models'

const currentYear = new Date().getFullYear()

test('[convertStringToModel] no date and time', () => {
    const result = TaskConverter.convertStringToModel('Test!')

    expect(result.title).toBe('Test!')
    expect(result.dateTime).toBe(null)
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

test('[convertStringToModel] date and no time', () => {
    const result = TaskConverter.convertStringToModel('1231 Test!')

    expect(result.title).toBe('Test!')
    expect(result.dateTime!.getTime())
        .toBe(new Date(currentYear, 11, 31, 0, 0, 0).getTime())
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

test('[convertStringToModel] date and no time 2 - not working w/o space', () => {
    const result = TaskConverter.convertStringToModel('0101Test!!!')

    expect(result.title).toBe('0101Test!!!')
    expect(result.dateTime).toBe(null)
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

test('[convertStringToModel] date and time', () => {
    const result = TaskConverter.convertStringToModel('1231 2359 Test!')

    expect(result.title).toBe('Test!')
    expect(result.dateTime!.getTime())
        .toBe(new Date(currentYear, 11, 31, 23, 59, 0).getTime())
    expect(result.timeType).toBe(TaskTimeTypeEnum.ConcreteTime)
})

test('[convertStringToModel] date and time 2 - not working w/o space', () => {
    const result = TaskConverter.convertStringToModel('0101 0101Test!!!')

    expect(result.title).toBe('0101Test!!!')
    expect(result.dateTime!.getTime())
        .toBe(new Date(currentYear, 0, 1, 0, 0, 0).getTime())
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

test('[convertStringToModel] date and no time with year', () => {
    const result = TaskConverter.convertStringToModel('20170101 Test')

    expect(result.title).toBe('Test')
    expect(result.dateTime!.getTime())
        .toBe(new Date(2017, 0, 1, 0, 0, 0).getTime())
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

test('[convertStringToModel] is probable', () => {
    const result = TaskConverter.convertStringToModel('Test! ?')

    expect(result.title).toBe('Test!')
    expect(result.isProbable).toBe(true)
})

test('[convertStringToModel] all day long with short date', () => {
    const result = TaskConverter.convertStringToModel('0220! Test')

    expect(result.title).toBe('Test')
    expect(result.dateTime!.getTime())
        .toBe(new Date(currentYear, 1, 20, 0, 0, 0).getTime())
    expect(result.timeType).toBe(TaskTimeTypeEnum.AllDayLong)
})

test('[convertStringToModel] all day long with long date', () => {
    const result = TaskConverter.convertStringToModel('20150220! Test')

    expect(result.title).toBe('Test')
    expect(result.dateTime!.getTime())
        .toBe(new Date(2015, 1, 20, 0, 0, 0).getTime())
    expect(result.timeType).toBe(TaskTimeTypeEnum.AllDayLong)
})

test('[convertStringToModel] simple text starting with exclamation mark', () => {
    const result = TaskConverter.convertStringToModel('! Test')

    expect(result.title).toBe('! Test')
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

test('[convertStringToModel] ignore time if all day long', () => {
    const result = TaskConverter.convertStringToModel('20150606! 2359 Test')

    expect(result.title).toBe('2359 Test')
    expect(result.dateTime!.getTime())
        .toBe(new Date(2015, 5, 6, 0, 0, 0).getTime())
    expect(result.timeType).toBe(TaskTimeTypeEnum.AllDayLong)
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

test('[convertModelToString] is probable', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!', null, TaskTimeTypeEnum.NoTime, true))
    expect(result).toBe('Test! ?')
})

test('[convertModelToString] all day long', () => {
    const result = TaskConverter.convertModelToString(new TaskModel('Test!', new Date(new Date().getFullYear(), 0, 1, 10, 10), TaskTimeTypeEnum.AllDayLong))
    expect(result).toBe('0101! Test!')
})
