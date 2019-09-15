import { TaskMoveService } from '../../services'
import { Task, TaskTypeEnum } from '../../models'

function task(taskId: number, taskDate: Date | null, taskOrder: number, type: TaskTypeEnum = TaskTypeEnum.NoTime, time: number | null = null): Task {
    const t = new Task(taskId, '', taskDate, taskOrder)
    t.type = type
    t.time = time
    return t
}

function d(year: number, month: number, date: number): Date {
    return new Date(year, month + 1, date)
}

function expectOrder(tasks: Task[], taskId: number, order: number) {
    expect(tasks.find(x => x.clientId === taskId)!.order).toBe(order)
}

function expectTime(tasks: Task[], taskId: number, time: number | null) {
    expect(tasks.find(x => x.clientId === taskId)!.time).toBe(time)
}

function expectDate(tasks: Task[], taskId: number, date: Date | null) {
    if (date === null) {
        expect(tasks.find(x => x.clientId === taskId)!.date).toBeNull()
    } else {
        expect(tasks.find(x => x.clientId === taskId)!.date!.getTime()).toBe(date.getTime())
    }
}

function expectChanged(tasks: Task[], taskId: number, changed: boolean) {
    expect(tasks.find(x => x.clientId === taskId)!.changed).toBe(changed)
}

test('positive', () => {
    const tasks: Task[] = [
        task(1, d(2018, 9, 9), 1),
        task(2, d(2018, 9, 9), 2),
        task(3, d(2018, 9, 9), 3),
        task(4, d(2018, 9, 10), 1),
        task(5, d(2018, 9, 10), 2)
    ]

    const result = TaskMoveService.moveTask(tasks, 4, d(2018, 9, 9).getTime(), d(2018, 9, 10).getTime(), 2)

    expectOrder(result, 1, 1)
    expectOrder(result, 4, 2)
    expectOrder(result, 2, 3)
    expectOrder(result, 3, 4)
    expectOrder(result, 5, 1)
    expectDate(result, 4, d(2018, 9, 9))
    expectChanged(result, 1, false)
    expectChanged(result, 2, true)
    expectChanged(result, 3, true)
    expectChanged(result, 4, true)
    expectChanged(result, 5, true)
})

test('move as last', () => {
    const tasks: Task[] = [
        task(1, d(2018, 9, 9), 1),
        task(2, d(2018, 9, 10), 1)
    ]

    const result = TaskMoveService.moveTask(tasks, 2, d(2018, 9, 9).getTime(), d(2018, 9, 10).getTime(), null)

    expectOrder(result, 1, 1)
    expectOrder(result, 2, 2)
    expectDate(result, 2, d(2018, 9, 9))
    expectChanged(result, 1, false)
    expectChanged(result, 2, true)
})

test('move as first', () => {
    const tasks: Task[] = [
        task(1, d(2018, 9, 9), 1),
        task(2, d(2018, 9, 10), 1)
    ]

    const result = TaskMoveService.moveTask(tasks, 2, d(2018, 9, 9).getTime(), d(2018, 9, 10).getTime(), 1)

    expectOrder(result, 1, 2)
    expectOrder(result, 2, 1)
    expectDate(result, 2, d(2018, 9, 9))
    expectChanged(result, 1, true)
    expectChanged(result, 2, true)
})

test('move inside the same list', () => {
    const tasks: Task[] = [
        task(1, d(2018, 9, 9), 1),
        task(2, d(2018, 9, 9), 2),
        task(3, d(2018, 9, 9), 3)
    ]

    const result = TaskMoveService.moveTask(tasks, 3, d(2018, 9, 9).getTime(), d(2018, 9, 9).getTime(), 2)

    expectOrder(result, 1, 1)
    expectOrder(result, 2, 3)
    expectOrder(result, 3, 2)
    expectDate(result, 3, d(2018, 9, 9))
    expectChanged(result, 1, false)
    expectChanged(result, 2, true)
    expectChanged(result, 3, true)
})

test('move to no date', () => {
    const tasks: Task[] = [
        task(1, null, 1),
        task(2, null, 2),
        task(3, d(2018, 9, 9), 1)
    ]

    const result = TaskMoveService.moveTask(tasks, 3, 0, d(2018, 9, 9).getTime(), 2)

    expectOrder(result, 1, 1)
    expectOrder(result, 2, 3)
    expectOrder(result, 3, 2)
    expectDate(result, 3, null)
    expectChanged(result, 1, false)
    expectChanged(result, 2, true)
    expectChanged(result, 3, true)
})

test('move with time', () => {
    const tasks: Task[] = [
        task(1, d(2018, 9, 10), 1, TaskTypeEnum.NoTime, 306)
    ]

    const result = TaskMoveService.moveTask(tasks, 1, d(2018, 9, 9).getTime(), d(2018, 9, 10).getTime(), null)

    expectOrder(result, 1, 1)
    expectDate(result, 1, d(2018, 9, 9))
    expectChanged(result, 1, true)
    expectTime(result, 1, 306)
})

test('any strange order becomes normal', () => {
    const tasks: Task[] = [
        task(1, d(2018, 9, 9), 12345),
        task(2, d(2018, 9, 9), 22222),
        task(3, d(2018, 9, 9), 98765),
        task(4, d(2018, 9, 10), 1000)
    ]

    const result = TaskMoveService.moveTask(tasks, 4, d(2018, 9, 9).getTime(), d(2018, 9, 10).getTime(), null)

    expectOrder(result, 1, 1)
    expectOrder(result, 2, 2)
    expectOrder(result, 3, 3)
    expectOrder(result, 4, 4)
    expectChanged(result, 1, true)
    expectChanged(result, 2, true)
    expectChanged(result, 3, true)
    expectChanged(result, 4, true)
})

test('ignore All Day Long tasks', () => {
    const tasks: Task[] = [
        task(1, d(2018, 9, 9), 1000, TaskTypeEnum.AllDayLong),
        task(2, d(2018, 9, 9), 1),
        task(3, d(2018, 9, 9), 2),
        task(4, d(2018, 9, 10), 1)
    ]

    const result = TaskMoveService.moveTask(tasks, 4, d(2018, 9, 9).getTime(), d(2018, 9, 10).getTime(), 2)

    expectOrder(result, 1, 1000)
    expectOrder(result, 2, 2)
    expectOrder(result, 3, 3)
    expectOrder(result, 4, 1)
    expectChanged(result, 1, false)
    expectChanged(result, 2, true)
    expectChanged(result, 3, true)
    expectChanged(result, 4, true)
})
