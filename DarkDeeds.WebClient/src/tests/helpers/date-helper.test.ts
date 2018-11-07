import { DateHelper } from '../../helpers'
import { Task } from '../../models'

test('[toDateFromSpecialFormat] positive', () => {
    const result = DateHelper.toDateFromSpecialFormat('20180704')
    expect(result!.getTime())
        .toBe(new Date(2018, 6, 4).getTime())
})

test('[toDateFromSpecialFormat] null if incorrect string', () => {
    expect(DateHelper.toDateFromSpecialFormat('123456789'))
        .toBe(null)

    expect(DateHelper.toDateFromSpecialFormat('1234567'))
        .toBe(null)

    expect(DateHelper.toDateFromSpecialFormat('1234!678'))
        .toBe(null)

    expect(DateHelper.toDateFromSpecialFormat('1234a678'))
        .toBe(null)

    expect(DateHelper.toDateFromSpecialFormat('1234+678'))
        .toBe(null)

    expect(DateHelper.toDateFromSpecialFormat('1234-678'))
        .toBe(null)
})

test('[getWeekdayName] positive', () => {
    expect(DateHelper.getWeekdayName(new Date(2018, 9, 21)))
        .toBe('Sunday')
})

test('[toLabel] positive', () => {
    expect(DateHelper.toLabel(new Date(2018, 9, 21)))
        .toBe('10/21/2018 Sunday')
})

test('[monday] positive', () => {
    expect(DateHelper.monday(new Date(2018, 9, 17)).getTime())
        .toBe(new Date(2018, 9, 15).getTime())
})

test('[dayStart] positive', () => {
    expect(DateHelper.dayStart(new Date(2018, 9, 17, 10, 10, 10)).getTime())
        .toBe(new Date(2018, 9, 17).getTime())
})

test('[fixDates] positive', () => {
    const arr: any = [new Task(1, ''), { id: 2, dateTime: '2018-11-14T22:00:00+04:00' }]
    const result = DateHelper.fixDates(arr) as Task[]

    expect(result).not.toBe(arr)
    expect(result.find(x => x.id === 1)!.dateTime).toBeNull()
    expect(result.find(x => x.id === 2)!.dateTime!.getTime())
        .toBe(new Date(2018, 10, 14, 22).getTime())
})

test('[equalDatesByStart] positive', () => {
    expect(DateHelper.equalDatesByStart(new Date(2018, 9, 17, 10, 10, 10), new Date(2018, 9, 17, 12, 12, 12))).toBeTruthy()
})
