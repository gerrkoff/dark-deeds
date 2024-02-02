import { DateService } from 'di/services/date-service'
import { TaskService } from 'di/services/task-service'
import { Task, TaskTypeEnum } from 'models'

function task(
    year: number,
    month: number,
    date: number,
    uid: string = '0',
    order: number = 0,
    timeType: TaskTypeEnum = TaskTypeEnum.Simple,
    hours: number = 0,
    minutes: number = 0
): Task {
    return new Task(
        uid,
        '',
        new Date(year, month, date, hours, minutes),
        order,
        false,
        false,
        false,
        timeType
    )
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
        new Task('0', ''),
        new Task('0', ''),
    ]

    const dateServiceMock = {
        monday: jest.fn().mockImplementation(() => new Date(2018, 9, 15)),
        today: jest.fn(),
    }
    const service = new TaskService(dateServiceMock as unknown as DateService)
    const result = service.evalModel(tasks, true)

    expect(result.noDate.length).toBe(2)
    expect(result.expired.length).toBe(2)
    expect(result.current.length).toBe(14)
    expect(result.future.length).toBe(2)
})

test('[evalModel] ignore completed if showCompleted=false', () => {
    const tasks: Task[] = [
        new Task('1', '', null, 0, false, true),
        new Task('2', '', null, 0, false, false),
        new Task('3', '', null, 0, false, true),
    ]

    const dateServiceMock = {
        monday: jest.fn().mockImplementation(() => new Date(2018, 9, 15)),
        today: jest.fn(),
    }
    const service = new TaskService(dateServiceMock as unknown as DateService)
    const result = service.evalModel(tasks, false)

    expect(result.noDate.length).toBe(1)
    expect(result.noDate[0].uid).toBe('2')
})

test('[sort] positive', () => {
    const tasks = [
        new Task(
            '1',
            '',
            new Date(2018, 1, 1),
            4,
            false,
            false,
            false,
            TaskTypeEnum.Simple
        ),
        new Task(
            '2',
            '',
            new Date(2018, 1, 1),
            0,
            false,
            false,
            false,
            TaskTypeEnum.Simple,
            false,
            0,
            600
        ),
        new Task(
            '3',
            '',
            new Date(2018, 1, 1),
            11,
            false,
            false,
            false,
            TaskTypeEnum.Simple
        ),
        new Task(
            '4',
            '',
            new Date(2018, 1, 1),
            1,
            false,
            false,
            false,
            TaskTypeEnum.Simple,
            false,
            0,
            600
        ),
        new Task(
            '5',
            '',
            new Date(2018, 1, 1),
            10,
            false,
            false,
            false,
            TaskTypeEnum.Simple
        ),
        new Task(
            '6',
            '',
            new Date(2018, 1, 1),
            2,
            false,
            false,
            false,
            TaskTypeEnum.Simple,
            false,
            0,
            480
        ),
        new Task(
            '7',
            '',
            new Date(2018, 1, 1),
            7,
            false,
            false,
            false,
            TaskTypeEnum.Simple
        ),
        new Task(
            '8',
            '',
            new Date(2018, 1, 1),
            3,
            false,
            false,
            false,
            TaskTypeEnum.Simple,
            false,
            0,
            900
        ),
        new Task(
            '9',
            '',
            new Date(2018, 1, 1),
            8,
            false,
            false,
            false,
            TaskTypeEnum.Simple,
            false,
            0,
            480
        ),
        new Task(
            '10',
            '',
            new Date(2018, 1, 1),
            5,
            false,
            false,
            false,
            TaskTypeEnum.Simple,
            false,
            0,
            480
        ),
        new Task(
            '11',
            '',
            new Date(2018, 1, 1),
            9,
            false,
            false,
            false,
            TaskTypeEnum.Additional
        ),
        new Task(
            '12',
            '',
            new Date(2018, 1, 1),
            6,
            false,
            false,
            false,
            TaskTypeEnum.Additional
        ),
    ]

    const service = new TaskService(null as unknown as DateService)
    const result = tasks.sort(service.sorting)

    expect(result[0].uid).toBe('2')
    expect(result[1].uid).toBe('4')
    expect(result[2].uid).toBe('6')
    expect(result[3].uid).toBe('8')
    expect(result[4].uid).toBe('1')
    expect(result[5].uid).toBe('10')
    expect(result[6].uid).toBe('12')
    expect(result[7].uid).toBe('7')
    expect(result[8].uid).toBe('9')
    expect(result[9].uid).toBe('11')
    expect(result[10].uid).toBe('5')
    expect(result[11].uid).toBe('3')
})
