import { DateService } from '../../services'
import { Task } from '../../models'

test('[toDateFromSpecialFormat] positive', () => {
    const result = DateService.toDateFromSpecialFormat('20180704')
    expect(result!.getTime())
        .toBe(new Date(2018, 6, 4).getTime())
})

test('[toDateFromSpecialFormat] null if incorrect string', () => {
    expect(DateService.toDateFromSpecialFormat('123456789'))
        .toBe(null)

    expect(DateService.toDateFromSpecialFormat('1234567'))
        .toBe(null)

    expect(DateService.toDateFromSpecialFormat('1234!678'))
        .toBe(null)

    expect(DateService.toDateFromSpecialFormat('1234a678'))
        .toBe(null)

    expect(DateService.toDateFromSpecialFormat('1234+678'))
        .toBe(null)

    expect(DateService.toDateFromSpecialFormat('1234-678'))
        .toBe(null)
})

test('[getWeekdayName] positive', () => {
    expect(DateService.getWeekdayName(new Date(2018, 9, 21)))
        .toBe('Sun')
})

test('[toLabel] positive', () => {
    expect(DateService.toLabel(new Date(2018, 9, 21)))
        .toBe('10/21/2018 Sun')
})

test('[monday] positive', () => {
    expect(DateService.monday(new Date(2018, 9, 17)).getTime())
        .toBe(new Date(2018, 9, 15).getTime())
})

test('[fixDates] positive', () => {
    const arr: any = [new Task(1, ''), { clientId: 2, date: '2018-11-14T18:00:00Z' }]
    const result = DateService.fixDates(arr) as Task[]

    expect(result).not.toBe(arr)
    expect(result.find(x => x.clientId === 1)!.date).toBeNull()
    expect(result.find(x => x.clientId === 2)!.date!.getTime())
        .toBe(new Date(Date.UTC(2018, 10, 14, 18)).getTime())
})

test('[equal] positive', () => {
    expect(DateService.equal(new Date(2018, 9, 17, 12, 12, 12), new Date(2018, 9, 17, 12, 12, 12))).toBeTruthy()
})
