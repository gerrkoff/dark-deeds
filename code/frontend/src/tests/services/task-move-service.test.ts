import { TaskMoveService, taskMoveService } from 'di/services/task-move-service'
import { Task, TaskTypeEnum } from 'models'

function createService(): TaskMoveService {
    // integration tests
    return taskMoveService
}

function task(
    taskId: string,
    taskDate: Date | null,
    taskOrder: number,
    type: TaskTypeEnum = TaskTypeEnum.Simple,
    time: number | null = null
): Task {
    const t = new Task(taskId, '', taskDate, taskOrder)
    t.type = type
    t.time = time
    return t
}

function d(year: number, month: number, date: number): Date {
    return new Date(year, month + 1, date)
}

function expectOrder(tasks: Task[], taskId: string, order: number) {
    expect(tasks.find(x => x.uid === taskId)!.order).toBe(order)
}

function expectTime(tasks: Task[], taskId: string, time: number | null) {
    expect(tasks.find(x => x.uid === taskId)!.time).toBe(time)
}

function expectDate(tasks: Task[], taskId: string, date: Date | null) {
    if (date === null) {
        expect(tasks.find(x => x.uid === taskId)!.date).toBeNull()
    } else {
        expect(tasks.find(x => x.uid === taskId)!.date!.getTime()).toBe(
            date.getTime()
        )
    }
}

function expectChanged(tasks: Task[], taskId: string, changed: boolean) {
    expect(tasks.find(x => x.uid === taskId)!.changed).toBe(changed)
}

test('positive', () => {
    const tasks: Task[] = [
        task('1', d(2018, 9, 9), 1),
        task('2', d(2018, 9, 9), 2),
        task('3', d(2018, 9, 9), 3),
        task('4', d(2018, 9, 10), 1),
        task('5', d(2018, 9, 10), 2),
    ]

    const service = createService()
    const result = service.moveTask(
        tasks,
        '4',
        d(2018, 9, 9).getTime(),
        d(2018, 9, 10).getTime(),
        '2'
    )

    expectOrder(result, '1', 1)
    expectOrder(result, '4', 2)
    expectOrder(result, '2', 3)
    expectOrder(result, '3', 4)
    expectOrder(result, '5', 1)
    expectDate(result, '4', d(2018, 9, 9))
    expectChanged(result, '1', false)
    expectChanged(result, '2', true)
    expectChanged(result, '3', true)
    expectChanged(result, '4', true)
    expectChanged(result, '5', true)
})

test('move as last', () => {
    const tasks: Task[] = [
        task('1', d(2018, 9, 9), 1),
        task('2', d(2018, 9, 10), 1),
    ]

    const service = createService()
    const result = service.moveTask(
        tasks,
        '2',
        d(2018, 9, 9).getTime(),
        d(2018, 9, 10).getTime(),
        null
    )

    expectOrder(result, '1', 1)
    expectOrder(result, '2', 2)
    expectDate(result, '2', d(2018, 9, 9))
    expectChanged(result, '1', false)
    expectChanged(result, '2', true)
})

test('move as first', () => {
    const tasks: Task[] = [
        task('1', d(2018, 9, 9), 1),
        task('2', d(2018, 9, 10), 1),
    ]

    const service = createService()
    const result = service.moveTask(
        tasks,
        '2',
        d(2018, 9, 9).getTime(),
        d(2018, 9, 10).getTime(),
        '1'
    )

    expectOrder(result, '1', 2)
    expectOrder(result, '2', 1)
    expectDate(result, '2', d(2018, 9, 9))
    expectChanged(result, '1', true)
    expectChanged(result, '2', true)
})

test('move inside the same list', () => {
    const tasks: Task[] = [
        task('1', d(2018, 9, 9), 1),
        task('2', d(2018, 9, 9), 2),
        task('3', d(2018, 9, 9), 3),
    ]

    const service = createService()
    const result = service.moveTask(
        tasks,
        '3',
        d(2018, 9, 9).getTime(),
        d(2018, 9, 9).getTime(),
        '2'
    )

    expectOrder(result, '1', 1)
    expectOrder(result, '2', 3)
    expectOrder(result, '3', 2)
    expectDate(result, '3', d(2018, 9, 9))
    expectChanged(result, '1', false)
    expectChanged(result, '2', true)
    expectChanged(result, '3', true)
})

test('move to no date', () => {
    const tasks: Task[] = [
        task('1', null, 1),
        task('2', null, 2),
        task('3', d(2018, 9, 9), 1),
    ]

    const service = createService()
    const result = service.moveTask(tasks, '3', 0, d(2018, 9, 9).getTime(), '2')

    expectOrder(result, '1', 1)
    expectOrder(result, '2', 3)
    expectOrder(result, '3', 2)
    expectDate(result, '3', null)
    expectChanged(result, '1', false)
    expectChanged(result, '2', true)
    expectChanged(result, '3', true)
})

test('move with time', () => {
    const tasks: Task[] = [
        task('1', d(2018, 9, 10), 1, TaskTypeEnum.Simple, 306),
    ]

    const service = createService()
    const result = service.moveTask(
        tasks,
        '1',
        d(2018, 9, 9).getTime(),
        d(2018, 9, 10).getTime(),
        null
    )

    expectOrder(result, '1', 1)
    expectDate(result, '1', d(2018, 9, 9))
    expectChanged(result, '1', true)
    expectTime(result, '1', 306)
})

test('any strange order becomes normal', () => {
    const tasks: Task[] = [
        task('1', d(2018, 9, 9), 12345),
        task('2', d(2018, 9, 9), 22222),
        task('3', d(2018, 9, 9), 98765),
        task('4', d(2018, 9, 10), 1000),
    ]

    const service = createService()
    const result = service.moveTask(
        tasks,
        '4',
        d(2018, 9, 9).getTime(),
        d(2018, 9, 10).getTime(),
        null
    )

    expectOrder(result, '1', 1)
    expectOrder(result, '2', 2)
    expectOrder(result, '3', 3)
    expectOrder(result, '4', 4)
    expectChanged(result, '1', true)
    expectChanged(result, '2', true)
    expectChanged(result, '3', true)
    expectChanged(result, '4', true)
})

test('ignore additional tasks', () => {
    const tasks: Task[] = [
        task('1', d(2018, 9, 9), 1000, TaskTypeEnum.Additional),
        task('2', d(2018, 9, 9), 1),
        task('3', d(2018, 9, 9), 2),
        task('4', d(2018, 9, 10), 1),
    ]

    const service = createService()
    const result = service.moveTask(
        tasks,
        '4',
        d(2018, 9, 9).getTime(),
        d(2018, 9, 10).getTime(),
        '2'
    )

    expectOrder(result, '1', 1000)
    expectOrder(result, '2', 2)
    expectOrder(result, '3', 3)
    expectOrder(result, '4', 1)
    expectChanged(result, '1', false)
    expectChanged(result, '2', true)
    expectChanged(result, '3', true)
    expectChanged(result, '4', true)
})
