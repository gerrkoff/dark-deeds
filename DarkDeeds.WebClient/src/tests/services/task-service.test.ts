import { TaskService } from '../../services'
import { Task, TaskTimeTypeEnum } from '../../models'

function task(year: number, month: number, date: number, id: number = 0, order: number = 0, timeType: TaskTimeTypeEnum = TaskTimeTypeEnum.NoTime, hours: number = 0, minutes: number = 0): Task {
    return new Task(id, '', new Date(year, month, date, hours, minutes), order, false, 0, false, false, timeType)
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

    const result = TaskService.evalModel(tasks, new Date(2018, 9, 17), true)

    expect(result.noDate.length).toBe(2)
    expect(result.expired.length).toBe(2)
    expect(result.current.length).toBe(14)
    expect(result.future.length).toBe(2)
})

test('[evalModel] ignore completed if showCompleted=false', () => {
    const tasks: Task[] = [
        new Task(1, '', null, 0, false, 0, true),
        new Task(2, '', null, 0, false, 0, false),
        new Task(3, '', null, 0, false, 0, true)
    ]

    const result = TaskService.evalModel(tasks, new Date(2018, 9, 17), false)

    expect(result.noDate.length).toBe(1)
    expect(result.noDate[0].clientId).toBe(2)
})

test('[tasksEqual] positive', () => {
    expect(TaskService.tasksEqual(new Task(1, '1', null, 1, false, 1), new Task(1, '1', null, 1, false, 1))).toBeTruthy()
    expect(TaskService.tasksEqual(new Task(1, '1', null, 1, false, 1), new Task(1, '1', new Date(), 1, false, 1))).not.toBeTruthy()
    expect(TaskService.tasksEqual(new Task(1, '1', new Date(2018), 1, false, 1), new Task(1, '1', new Date(2018), 1, false, 1))).toBeTruthy()
    expect(TaskService.tasksEqual(new Task(1, '1', new Date(2018), 1, false, 1), new Task(2, '2', new Date(2019), 2, false, 2))).not.toBeTruthy()
    expect(TaskService.tasksEqual(new Task(1, '1', null, 1, false, 1, false, false, TaskTimeTypeEnum.NoTime, false), new Task(1, '1', null, 1, false, 1, false, false, TaskTimeTypeEnum.NoTime, true))).not.toBeTruthy()
})

test('[sort] positive', () => {
    const tasks = [
        new Task(1, '', new Date(2018, 1, 1), 1, false, 0, false, false, TaskTimeTypeEnum.NoTime),
        new Task(2, '', new Date(2018, 1, 1, 10), 0, false, 0, false, false, TaskTimeTypeEnum.ConcreteTime),
        new Task(3, '', new Date(2018, 1, 1), 4, false, 0, false, false, TaskTimeTypeEnum.NoTime),
        new Task(4, '', new Date(2018, 1, 1, 10), 0, false, 0, false, false, TaskTimeTypeEnum.ConcreteTime),
        new Task(5, '', new Date(2018, 1, 1), 3, false, 0, false, false, TaskTimeTypeEnum.NoTime),
        new Task(6, '', new Date(2018, 1, 1, 8), 0, false, 0, false, false, TaskTimeTypeEnum.ConcreteTime),
        new Task(7, '', new Date(2018, 1, 1), 2, false, 0, false, false, TaskTimeTypeEnum.NoTime),
        new Task(8, '', new Date(2018, 1, 1, 15), 0, false, 0, false, false, TaskTimeTypeEnum.ConcreteTime),
        new Task(9, '', new Date(2018, 1, 1, 8), 2, false, 0, false, false, TaskTimeTypeEnum.ConcreteTime),
        new Task(10, '', new Date(2018, 1, 1, 8), 1, false, 0, false, false, TaskTimeTypeEnum.ConcreteTime),
        new Task(11, '', new Date(2018, 1, 1, 8), 2, false, 0, false, false, TaskTimeTypeEnum.AllDayLong),
        new Task(12, '', new Date(2018, 1, 1, 8), 1, false, 0, false, false, TaskTimeTypeEnum.AllDayLong)
    ]
    const result = tasks.sort(TaskService.sorting)

    expect(result[0].clientId).toBe(2)
    expect(result[1].clientId).toBe(4)
    expect(result[2].clientId).toBe(6)
    expect(result[3].clientId).toBe(8)
    expect(result[4].clientId).toBe(1)
    expect(result[5].clientId).toBe(10)
    expect(result[6].clientId).toBe(12)
    expect(result[7].clientId).toBe(7)
    expect(result[8].clientId).toBe(9)
    expect(result[9].clientId).toBe(11)
    expect(result[10].clientId).toBe(5)
    expect(result[11].clientId).toBe(3)
})
