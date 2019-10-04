import { DateService } from '../../di'
import { Task } from '../../models'

test('[toDateFromSpecialFormat] positive', () => {
    const service = new DateService()
    const result = service.toDateFromSpecialFormat('20180704')
    expect(result!.getTime())
        .toBe(new Date(2018, 6, 4).getTime())
})

test('[toDateFromSpecialFormat] null if incorrect string', () => {
    const service = new DateService()
    expect(service.toDateFromSpecialFormat('123456789'))
        .toBe(null)

    expect(service.toDateFromSpecialFormat('1234567'))
        .toBe(null)

    expect(service.toDateFromSpecialFormat('1234!678'))
        .toBe(null)

    expect(service.toDateFromSpecialFormat('1234a678'))
        .toBe(null)

    expect(service.toDateFromSpecialFormat('1234+678'))
        .toBe(null)

    expect(service.toDateFromSpecialFormat('1234-678'))
        .toBe(null)
})

test('[getWeekdayName] positive', () => {
    const service = new DateService()
    expect(service.getWeekdayName(new Date(2018, 9, 21)))
        .toBe('Sun')
})

test('[toLabel] positive', () => {
    const service = new DateService()
    expect(service.toLabel(new Date(2018, 9, 21)))
        .toBe('10/21/2018 Sun')
})

test('[monday] positive', () => {
    const service = new DateService()
    expect(service.monday(new Date(2018, 9, 17)).getTime())
        .toBe(new Date(2018, 9, 15).getTime())
})

test('[adjustDatesAfterLoading] positive', () => {
    const arr: any = [new Task(1, ''), { clientId: 2, date: '2018-11-14T18:00:00Z' }]

    const service = new DateService()
    const result = service.adjustDatesAfterLoading(arr) as Task[]

    expect(result).not.toBe(arr)
    expect(result.find(x => x.clientId === 1)!.date).toBeNull()
    expect(result.find(x => x.clientId === 2)!.date!.getTime())
        .toBe(new Date(2018, 10, 14, 19).getTime())
})

test('[equal] positive', () => {
    const service = new DateService()
    expect(service.equal(new Date(2018, 9, 17, 12, 12, 12), new Date(2018, 9, 17, 12, 12, 12))).toBeTruthy()
})
