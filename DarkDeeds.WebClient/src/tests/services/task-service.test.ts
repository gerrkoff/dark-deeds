import { di, diToken, DateService, TaskService } from '../../di'
import { Task, TaskTypeEnum } from '../../models'

function task(year: number, month: number, date: number, id: number = 0, order: number = 0, timeType: TaskTypeEnum = TaskTypeEnum.Simple, hours: number = 0, minutes: number = 0): Task {
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

    const service = di.get<TaskService>(diToken.TaskService)
    const result = service.evalModel(tasks, new Date(2018, 9, 17), true)

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

    const service = di.get<TaskService>(diToken.TaskService)
    const result = service.evalModel(tasks, new Date(2018, 9, 17), false)

    expect(result.noDate.length).toBe(1)
    expect(result.noDate[0].clientId).toBe(2)
})

test('[tasksEqual] positive', () => {
    const service = di.get<TaskService>(diToken.TaskService)
    expect(service.tasksEqual(new Task(1, '1', null, 1, false, 1), new Task(1, '1', null, 1, false, 1))).toBeTruthy()
    expect(service.tasksEqual(new Task(1, '1', null, 1, false, 1), new Task(1, '1', new Date(), 1, false, 1))).not.toBeTruthy()
    expect(service.tasksEqual(new Task(1, '1', new Date(2018), 1, false, 1), new Task(1, '1', new Date(2018), 1, false, 1))).toBeTruthy()
    expect(service.tasksEqual(new Task(1, '1', new Date(2018), 1, false, 1), new Task(2, '2', new Date(2019), 2, false, 2))).not.toBeTruthy()
    expect(service.tasksEqual(new Task(1, '1', null, 1, false, 1, false, false, TaskTypeEnum.Simple, false), new Task(1, '1', null, 1, false, 1, false, false, TaskTypeEnum.Simple, true))).not.toBeTruthy()
})

test('[sort] positive', () => {
    const tasks = [
        new Task(1, '', new Date(2018, 1, 1), 1, false, 0, false, false, TaskTypeEnum.Simple),
        new Task(2, '', new Date(2018, 1, 1), 0, false, 0, false, false, TaskTypeEnum.Simple, false, 0, 600),
        new Task(3, '', new Date(2018, 1, 1), 4, false, 0, false, false, TaskTypeEnum.Simple),
        new Task(4, '', new Date(2018, 1, 1), 0, false, 0, false, false, TaskTypeEnum.Simple, false, 0, 600),
        new Task(5, '', new Date(2018, 1, 1), 3, false, 0, false, false, TaskTypeEnum.Simple),
        new Task(6, '', new Date(2018, 1, 1), 0, false, 0, false, false, TaskTypeEnum.Simple, false, 0, 480),
        new Task(7, '', new Date(2018, 1, 1), 2, false, 0, false, false, TaskTypeEnum.Simple),
        new Task(8, '', new Date(2018, 1, 1), 0, false, 0, false, false, TaskTypeEnum.Simple, false, 0, 900),
        new Task(9, '', new Date(2018, 1, 1), 2, false, 0, false, false, TaskTypeEnum.Simple, false, 0, 480),
        new Task(10, '', new Date(2018, 1, 1), 1, false, 0, false, false, TaskTypeEnum.Simple, false, 0, 480),
        new Task(11, '', new Date(2018, 1, 1), 2, false, 0, false, false, TaskTypeEnum.Additional),
        new Task(12, '', new Date(2018, 1, 1), 1, false, 0, false, false, TaskTypeEnum.Additional)
    ]

    const service = new TaskService(jest.fn<DateService>() as unknown as DateService)
    const result = tasks.sort(service.sorting)

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
