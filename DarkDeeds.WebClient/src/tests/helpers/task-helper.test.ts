import { TaskHelper } from '../../helpers'
import { Task, TaskModel } from '../../models'

function task(year: number, month: number, date: number, id: number = 0, order: number = 0): Task {
    return new Task(id, '', new Date(year, month, date), order)
}

test('[evalModel] positive', () => {
    const tasks: Task[] = [
        task(2018, 9, 9),
        task(2018, 9, 9),
        task(2018, 9, 14),
        task(2018, 9, 15),
        task(2018, 9, 19),
        task(2018, 9, 23),
        task(2018, 9, 28),
        task(2018, 9, 29),
        task(2018, 10, 1),
        task(2018, 10, 1),
        new Task(0, ''),
        new Task(0, '')
    ]

    const result = TaskHelper.evalModel(tasks, new Date(2018, 9, 17))

    expect(result.noDate.length).toBe(2)
    expect(result.expired.length).toBe(2)
    expect(result.current.length).toBe(14)
    expect(result.future.length).toBe(2)
})

test('[moveTask] positive', () => {
    const tasks: Task[] = [
        task(2018, 9, 9, 1, 1),
        task(2018, 9, 9, 2, 2),
        task(2018, 9, 9, 3, 3),
        task(2018, 9, 10, 4, 1),
        task(2018, 9, 10, 5, 2)
    ]

    const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), 2)

    expect(result.find(x => x.clientId === 1)!.order).toBe(1)
    expect(result.find(x => x.clientId === 2)!.order).toBe(3)
    expect(result.find(x => x.clientId === 3)!.order).toBe(4)
    expect(result.find(x => x.clientId === 4)!.order).toBe(2)
    expect(result.find(x => x.clientId === 5)!.order).toBe(1)

    expect(result.find(x => x.clientId === 4)!.dateTime!.getTime()).toBe(new Date(2018, 9, 9).getTime())
})

test('[moveTask] move as last', () => {
    const tasks: Task[] = [
        task(2018, 9, 9, 1, 10),
        task(2018, 9, 9, 2, 2),
        task(2018, 9, 10, 3, 1)
    ]

    const result = TaskHelper.moveTask(tasks, 3, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), null)

    expect(result.find(x => x.clientId === 1)!.order).toBe(10)
    expect(result.find(x => x.clientId === 2)!.order).toBe(2)
    expect(result.find(x => x.clientId === 3)!.order).toBe(11)
})

test('[moveTask] same list reorder', () => {
    const tasks: Task[] = [
        task(2018, 9, 9, 1, 4),
        task(2018, 9, 9, 2, 3),
        task(2018, 9, 9, 3, 2),
        task(2018, 9, 9, 4, 1)
    ]

    const result = TaskHelper.moveTask(tasks, 2, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 9).getTime(), 4)

    expect(result.find(x => x.clientId === 1)!.order).toBe(4)
    expect(result.find(x => x.clientId === 2)!.order).toBe(1)
    expect(result.find(x => x.clientId === 3)!.order).toBe(3)
    expect(result.find(x => x.clientId === 4)!.order).toBe(2)
})

test('[moveTask] same list as last', () => {
    const tasks: Task[] = [
        task(2018, 9, 9, 1, 4),
        task(2018, 9, 9, 2, 3),
        task(2018, 9, 9, 3, 2),
        task(2018, 9, 9, 4, 1)
    ]

    const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 9).getTime(), null)

    expect(result.find(x => x.clientId === 1)!.order).toBe(3)
    expect(result.find(x => x.clientId === 2)!.order).toBe(2)
    expect(result.find(x => x.clientId === 3)!.order).toBe(1)
    expect(result.find(x => x.clientId === 4)!.order).toBe(4)
})

test('[createTaskFromText] no date and time', () => {
    const result = TaskHelper.createTaskFromText('Test!')

    expect(result.title).toBe('Test!')
    expect(result.dateTime).toBe(null)
})

test('[createTaskFromText] date and no time', () => {
    const result = TaskHelper.createTaskFromText('1231 Test!')
    const currentYear = new Date().getFullYear()

    expect(result.title).toBe('Test!')
    expect(result.dateTime!.getTime())
        .toBe(new Date(currentYear, 11, 31, 0, 0, 0).getTime())
})

test('[createTaskFromText] date and no time 2', () => {
    const result = TaskHelper.createTaskFromText('0101Test!!!')
    const currentYear = new Date().getFullYear()

    expect(result.title).toBe('Test!!!')
    expect(result.dateTime!.getTime())
        .toBe(new Date(currentYear, 0, 1, 0, 0, 0).getTime())
})

test('[createTaskFromText] date and time', () => {
    const result = TaskHelper.createTaskFromText('1231 2359 Test!')
    const currentYear = new Date().getFullYear()

    expect(result.title).toBe('Test!')
    expect(result.dateTime!.getTime())
        .toBe(new Date(currentYear, 11, 31, 23, 59, 0).getTime())
})

test('[createTaskFromText] date and time 2', () => {
    const result = TaskHelper.createTaskFromText('0101 0101Test!!!')
    const currentYear = new Date().getFullYear()

    expect(result.title).toBe('Test!!!')
    expect(result.dateTime!.getTime())
        .toBe(new Date(currentYear, 0, 1, 1, 1, 0).getTime())
})

test('[createTaskFromText] no date', () => {
    const result = TaskHelper.convertModelToString(new TaskModel('Test!'))
    expect(result).toBe('Test!')
})

test('[createTaskFromText] date & no time', () => {
    const result = TaskHelper.convertModelToString(new TaskModel('Test!', new Date(2018, 11, 11)))
    expect(result).toBe('1211 Test!')
})

test('[createTaskFromText] date & time', () => {
    const result = TaskHelper.convertModelToString(new TaskModel('Test!', new Date(2018, 11, 11, 23, 59), true))
    expect(result).toBe('1211 2359 Test!')
})

test('[createTaskFromText] date & time less ten', () => {
    const result = TaskHelper.convertModelToString(new TaskModel('Test!', new Date(2018, 0, 1, 1, 1), true))
    expect(result).toBe('0101 0101 Test!')
})
