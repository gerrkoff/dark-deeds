import { TaskHelper } from '../../helpers'
import { Task, TaskModel, TaskTimeTypeEnum } from '../../models'

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

test('[moveTask] move concrete time', () => {
    const tasks: Task[] = [
        task(2018, 9, 9, 1, 1),
        task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 11),
        task(2018, 9, 9, 3, 2),
        task(2018, 9, 10, 4, 0, TaskTimeTypeEnum.ConcreteTime, 12),
        task(2018, 9, 10, 5, 1)
    ]

    const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), 1)

    expect(result.find(x => x.clientId === 1)!.order).toBe(1)
    expect(result.find(x => x.clientId === 2)!.order).toBe(0)
    expect(result.find(x => x.clientId === 3)!.order).toBe(2)
    expect(result.find(x => x.clientId === 4)!.order).toBe(0)
    expect(result.find(x => x.clientId === 5)!.order).toBe(1)

    expect(result.find(x => x.clientId === 4)!.dateTime!.getTime()).toBe(new Date(2018, 9, 9, 12).getTime())
})

test('[moveTask] move after time - from after time to after time', () => {
    const tasks: Task[] = [
        task(2018, 9, 9, 1, 1),
        task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 12, 0),
        task(2018, 9, 9, 3, 2),
        task(2018, 9, 10, 4, 0, TaskTimeTypeEnum.AfterTime, 14, 5),
        task(2018, 9, 10, 5, 1)
    ]

    const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), null)

    expect(result.find(x => x.clientId === 1)!.order).toBe(1)
    expect(result.find(x => x.clientId === 2)!.order).toBe(0)
    expect(result.find(x => x.clientId === 3)!.order).toBe(2)
    expect(result.find(x => x.clientId === 5)!.order).toBe(1)

    const movedTask = result.find(x => x.clientId === 4)!
    expect(movedTask.order).toBe(1)
    expect(movedTask.dateTime!.getTime()).toBe(new Date(2018, 9, 9, 12).getTime())
    expect(movedTask.timeType).toBe(TaskTimeTypeEnum.AfterTime)
})

test('[moveTask] move after time - from no time to after time', () => {
    const tasks: Task[] = [
        task(2018, 9, 9, 1, 1),
        task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 12),
        task(2018, 9, 9, 3, 0, TaskTimeTypeEnum.ConcreteTime, 13),
        task(2018, 9, 10, 4, 1),
        task(2018, 9, 10, 5, 2)
    ]

    const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), 3)

    expect(result.find(x => x.clientId === 1)!.order).toBe(1)
    expect(result.find(x => x.clientId === 2)!.order).toBe(0)
    expect(result.find(x => x.clientId === 3)!.order).toBe(0)
    expect(result.find(x => x.clientId === 5)!.order).toBe(1)

    const movedTask = result.find(x => x.clientId === 4)!
    expect(movedTask.order).toBe(1)
    expect(movedTask.dateTime!.getTime()).toBe(new Date(2018, 9, 9, 12).getTime())
    expect(movedTask.timeType).toBe(TaskTimeTypeEnum.AfterTime)
})

test('[moveTask] move after time - from after time to no time', () => {
    const tasks: Task[] = [
        task(2018, 9, 9, 1, 1),
        task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 12),
        task(2018, 9, 10, 3, 0, TaskTimeTypeEnum.ConcreteTime, 13),
        task(2018, 9, 10, 4, 1, TaskTimeTypeEnum.AfterTime, 13),
        task(2018, 9, 10, 5, 1)
    ]

    const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), 2)

    expect(result.find(x => x.clientId === 1)!.order).toBe(1)
    expect(result.find(x => x.clientId === 2)!.order).toBe(0)
    expect(result.find(x => x.clientId === 3)!.order).toBe(0)
    expect(result.find(x => x.clientId === 5)!.order).toBe(1)

    const movedTask = result.find(x => x.clientId === 4)!
    expect(movedTask.order).toBe(2)
    expect(movedTask.dateTime!.getTime()).toBe(new Date(2018, 9, 9).getTime())
    expect(movedTask.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

test('[moveTask] moving to no date (reset type)', () => {
    const tasks: Task[] = [
        task(2018, 9, 9, 1, 1),
        task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 12),
        task(2018, 9, 10, 3, 0, TaskTimeTypeEnum.ConcreteTime, 13),
        task(2018, 9, 10, 4, 1, TaskTimeTypeEnum.AfterTime, 13),
        task(2018, 9, 10, 5, 2, TaskTimeTypeEnum.AfterTime, 13)
    ]

    const result = TaskHelper.moveTask(tasks, 4, 0, new Date(2018, 9, 10).getTime(), null)

    expect(result.find(x => x.clientId === 1)!.order).toBe(1)
    expect(result.find(x => x.clientId === 2)!.order).toBe(0)
    expect(result.find(x => x.clientId === 3)!.order).toBe(0)
    expect(result.find(x => x.clientId === 5)!.order).toBe(1)

    const movedTask = result.find(x => x.clientId === 4)!
    expect(movedTask.order).toBe(1)
    expect(movedTask.dateTime).toBeNull()
    expect(movedTask.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

test('[moveTask] move concrete time and adjust source aftertime to be no time', () => {
    const tasks: Task[] = [
        task(2018, 9, 9, 1, 1),
        task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 11),
        task(2018, 9, 9, 3, 2),
        task(2018, 9, 10, 4, 0, TaskTimeTypeEnum.ConcreteTime, 12),
        task(2018, 9, 10, 5, 1),
        task(2018, 9, 10, 6, 1, TaskTimeTypeEnum.AfterTime, 12),
        task(2018, 9, 10, 7, 2, TaskTimeTypeEnum.AfterTime, 12)
    ]

    const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), 1)

    expect(result.find(x => x.clientId === 1)!.order).toBe(1)
    expect(result.find(x => x.clientId === 2)!.order).toBe(0)
    expect(result.find(x => x.clientId === 3)!.order).toBe(2)
    expect(result.find(x => x.clientId === 4)!.order).toBe(0)
    expect(result.find(x => x.clientId === 5)!.order).toBe(1)

    const taskRes1 = result.find(x => x.clientId === 6)!
    expect(taskRes1.order).toBe(2)
    expect(taskRes1.dateTime!.getTime()).toBe(new Date(2018, 9, 10).getTime())
    expect(taskRes1.timeType).toBe(TaskTimeTypeEnum.NoTime)

    const taskRes2 = result.find(x => x.clientId === 7)!
    expect(taskRes2.order).toBe(3)
    expect(taskRes2.dateTime!.getTime()).toBe(new Date(2018, 9, 10).getTime())
    expect(taskRes2.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

test('[moveTask] move concrete time and adjust source aftertime to be after time (concrete time)', () => {
    const tasks: Task[] = [
        task(2018, 9, 9, 1, 1),
        task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 11),
        task(2018, 9, 9, 3, 2),
        task(2018, 9, 10, 4, 0, TaskTimeTypeEnum.ConcreteTime, 12),
        task(2018, 9, 10, 5, 0, TaskTimeTypeEnum.ConcreteTime, 11),
        task(2018, 9, 10, 6, 1, TaskTimeTypeEnum.AfterTime, 12),
        task(2018, 9, 10, 7, 2, TaskTimeTypeEnum.AfterTime, 12)
    ]

    const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), 1)

    expect(result.find(x => x.clientId === 1)!.order).toBe(1)
    expect(result.find(x => x.clientId === 2)!.order).toBe(0)
    expect(result.find(x => x.clientId === 3)!.order).toBe(2)
    expect(result.find(x => x.clientId === 4)!.order).toBe(0)
    expect(result.find(x => x.clientId === 5)!.order).toBe(0)

    const taskRes1 = result.find(x => x.clientId === 6)!
    expect(taskRes1.order).toBe(1)
    expect(taskRes1.dateTime!.getTime()).toBe(new Date(2018, 9, 10, 11).getTime())
    expect(taskRes1.timeType).toBe(TaskTimeTypeEnum.AfterTime)

    const taskRes2 = result.find(x => x.clientId === 7)!
    expect(taskRes2.order).toBe(2)
    expect(taskRes2.dateTime!.getTime()).toBe(new Date(2018, 9, 10, 11).getTime())
    expect(taskRes2.timeType).toBe(TaskTimeTypeEnum.AfterTime)
})

test('[moveTask] move concrete time and adjust source aftertime to be after time (after time)', () => {
    const tasks: Task[] = [
        task(2018, 9, 9, 1, 1),
        task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 11),
        task(2018, 9, 9, 3, 2),
        task(2018, 9, 10, 4, 0, TaskTimeTypeEnum.ConcreteTime, 12),
        task(2018, 9, 10, 5, 0, TaskTimeTypeEnum.ConcreteTime, 11),
        task(2018, 9, 10, 6, 1, TaskTimeTypeEnum.AfterTime, 12),
        task(2018, 9, 10, 7, 2, TaskTimeTypeEnum.AfterTime, 12),
        task(2018, 9, 10, 8, 1, TaskTimeTypeEnum.AfterTime, 11)
    ]

    const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), 1)

    expect(result.find(x => x.clientId === 1)!.order).toBe(1)
    expect(result.find(x => x.clientId === 2)!.order).toBe(0)
    expect(result.find(x => x.clientId === 3)!.order).toBe(2)
    expect(result.find(x => x.clientId === 4)!.order).toBe(0)
    expect(result.find(x => x.clientId === 5)!.order).toBe(0)
    expect(result.find(x => x.clientId === 8)!.order).toBe(1)

    const taskRes1 = result.find(x => x.clientId === 6)!
    expect(taskRes1.order).toBe(2)
    expect(taskRes1.dateTime!.getTime()).toBe(new Date(2018, 9, 10, 11).getTime())
    expect(taskRes1.timeType).toBe(TaskTimeTypeEnum.AfterTime)

    const taskRes2 = result.find(x => x.clientId === 7)!
    expect(taskRes2.order).toBe(3)
    expect(taskRes2.dateTime!.getTime()).toBe(new Date(2018, 9, 10, 11).getTime())
    expect(taskRes2.timeType).toBe(TaskTimeTypeEnum.AfterTime)
})

test('[moveTask] move concrete time and no adjust if source and target are the same', () => {
    const tasks: Task[] = [
        task(2018, 9, 10, 4, 0, TaskTimeTypeEnum.ConcreteTime, 12),
        task(2018, 9, 10, 8, 1, TaskTimeTypeEnum.AfterTime, 12)
    ]

    const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 10).getTime(), new Date(2018, 9, 10).getTime(), null)

    expect(result.find(x => x.clientId === 4)!.order).toBe(0)

    const taskRes = result.find(x => x.clientId === 8)!
    expect(taskRes.order).toBe(1)
    expect(taskRes.dateTime!.getTime()).toBe(new Date(2018, 9, 10, 12).getTime())
    expect(taskRes.timeType).toBe(TaskTimeTypeEnum.AfterTime)
})

test('[moveTask] same list no date 1', () => {
    const tasks: Task[] = [
        new Task(1, '', null, 1, false, 0, false, false, TaskTimeTypeEnum.NoTime),
        new Task(2, '', null, 2, false, 0, false, false, TaskTimeTypeEnum.NoTime),
        new Task(3, '', null, 4, false, 0, false, false, TaskTimeTypeEnum.NoTime),
        new Task(4, '', null, 3, false, 0, false, false, TaskTimeTypeEnum.NoTime)
    ]

    const result = TaskHelper.moveTask(tasks, 4, 0, 0, 1)

    expect(result.find(x => x.clientId === 1)!.order).toBe(2)
    expect(result.find(x => x.clientId === 2)!.order).toBe(3)
    expect(result.find(x => x.clientId === 3)!.order).toBe(4)
    expect(result.find(x => x.clientId === 4)!.order).toBe(1)
})

test('[moveTask] same list no date 2', () => {
    const tasks: Task[] = [
        new Task(1, '', null, 1, false, 0, false, false, TaskTimeTypeEnum.NoTime),
        new Task(2, '', null, 2, false, 0, false, false, TaskTimeTypeEnum.NoTime),
        new Task(3, '', null, 4, false, 0, false, false, TaskTimeTypeEnum.NoTime),
        new Task(4, '', null, 3, false, 0, false, false, TaskTimeTypeEnum.NoTime)
    ]

    const result = TaskHelper.moveTask(tasks, 3, 0, 0, 4)

    expect(result.find(x => x.clientId === 1)!.order).toBe(1)
    expect(result.find(x => x.clientId === 2)!.order).toBe(2)
    expect(result.find(x => x.clientId === 3)!.order).toBe(3)
    expect(result.find(x => x.clientId === 4)!.order).toBe(4)
})

test('[convertStringToModel] no date and time', () => {
    const result = TaskHelper.convertStringToModel('Test!')

    expect(result.title).toBe('Test!')
    expect(result.dateTime).toBe(null)
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

test('[convertStringToModel] date and no time', () => {
    const result = TaskHelper.convertStringToModel('1231 Test!')
    const currentYear = new Date().getFullYear()

    expect(result.title).toBe('Test!')
    expect(result.dateTime!.getTime())
        .toBe(new Date(currentYear, 11, 31, 0, 0, 0).getTime())
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

test('[convertStringToModel] date and no time 2 - not working w/o space', () => {
    const result = TaskHelper.convertStringToModel('0101Test!!!')

    expect(result.title).toBe('0101Test!!!')
    expect(result.dateTime).toBe(null)
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

test('[convertStringToModel] date and time', () => {
    const result = TaskHelper.convertStringToModel('1231 2359 Test!')
    const currentYear = new Date().getFullYear()

    expect(result.title).toBe('Test!')
    expect(result.dateTime!.getTime())
        .toBe(new Date(currentYear, 11, 31, 23, 59, 0).getTime())
    expect(result.timeType).toBe(TaskTimeTypeEnum.ConcreteTime)
})

test('[convertStringToModel] date and time 2 - not working w/o space', () => {
    const result = TaskHelper.convertStringToModel('0101 0101Test!!!')
    const currentYear = new Date().getFullYear()

    expect(result.title).toBe('0101Test!!!')
    expect(result.dateTime!.getTime())
        .toBe(new Date(currentYear, 0, 1, 0, 0, 0).getTime())
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

test('[convertStringToModel] date and no time with year', () => {
    const result = TaskHelper.convertStringToModel('20170101 Test')

    expect(result.title).toBe('Test')
    expect(result.dateTime!.getTime())
        .toBe(new Date(2017, 0, 1, 0, 0, 0).getTime())
    expect(result.timeType).toBe(TaskTimeTypeEnum.NoTime)
})

test('[convertStringToModel] date and after time with year', () => {
    const result = TaskHelper.convertStringToModel('20171010 >0211 Test')

    expect(result.title).toBe('Test')
    expect(result.dateTime!.getTime())
        .toBe(new Date(2017, 9, 10, 2, 11, 0).getTime())
    expect(result.timeType).toBe(TaskTimeTypeEnum.AfterTime)
})

test('[convertModelToString] no date', () => {
    const result = TaskHelper.convertModelToString(new TaskModel('Test!'))
    expect(result).toBe('Test!')
})

test('[convertModelToString] date & no time', () => {
    const result = TaskHelper.convertModelToString(new TaskModel('Test!', new Date(2018, 11, 11)))
    expect(result).toBe('1211 Test!')
})

test('[convertModelToString] date & time', () => {
    const result = TaskHelper.convertModelToString(new TaskModel('Test!', new Date(2018, 11, 11, 23, 59), TaskTimeTypeEnum.ConcreteTime))
    expect(result).toBe('1211 2359 Test!')
})

test('[convertModelToString] date & time less ten', () => {
    const result = TaskHelper.convertModelToString(new TaskModel('Test!', new Date(2018, 0, 1, 1, 1), TaskTimeTypeEnum.AfterTime))
    expect(result).toBe('0101 >0101 Test!')
})

test('[tasksEqual] positive', () => {
    expect(TaskHelper.tasksEqual(new Task(1, '1', null, 1, false, 1), new Task(1, '1', null, 1, false, 1))).toBeTruthy()
    expect(TaskHelper.tasksEqual(new Task(1, '1', null, 1, false, 1), new Task(1, '1', new Date(), 1, false, 1))).not.toBeTruthy()
    expect(TaskHelper.tasksEqual(new Task(1, '1', new Date(2018), 1, false, 1), new Task(1, '1', new Date(2018), 1, false, 1))).toBeTruthy()
    expect(TaskHelper.tasksEqual(new Task(1, '1', new Date(2018), 1, false, 1), new Task(2, '2', new Date(2019), 2, false, 2))).not.toBeTruthy()
})

test('[sortTasks] positive', () => {
    const tasks = [
        new Task(1, '', new Date(2018, 1, 1), 1, false, 0, false, false, TaskTimeTypeEnum.NoTime),
        new Task(8, '', new Date(2018, 1, 1, 10), 0, false, 0, false, false, TaskTimeTypeEnum.AfterTime),
        new Task(2, '', new Date(2018, 1, 1), 4, false, 0, false, false, TaskTimeTypeEnum.NoTime),
        new Task(3, '', new Date(2018, 1, 1, 10), 0, false, 0, false, false, TaskTimeTypeEnum.ConcreteTime),
        new Task(4, '', new Date(2018, 1, 1), 3, false, 0, false, false, TaskTimeTypeEnum.NoTime),
        new Task(5, '', new Date(2018, 1, 1, 8), 0, false, 0, false, false, TaskTimeTypeEnum.ConcreteTime),
        new Task(6, '', new Date(2018, 1, 1), 2, false, 0, false, false, TaskTimeTypeEnum.NoTime),
        new Task(7, '', new Date(2018, 1, 1, 15), 0, false, 0, false, false, TaskTimeTypeEnum.ConcreteTime),
        new Task(9, '', new Date(2018, 1, 1, 8), 2, false, 0, false, false, TaskTimeTypeEnum.AfterTime),
        new Task(10, '', new Date(2018, 1, 1, 8), 1, false, 0, false, false, TaskTimeTypeEnum.AfterTime)
    ]
    const result = TaskHelper.sortTasks(tasks)

    expect(result[0].clientId).toBe(1)
    expect(result[1].clientId).toBe(6)
    expect(result[2].clientId).toBe(4)
    expect(result[3].clientId).toBe(2)
    expect(result[4].clientId).toBe(5)
    expect(result[5].clientId).toBe(10)
    expect(result[6].clientId).toBe(9)
    expect(result[7].clientId).toBe(3)
    expect(result[8].clientId).toBe(8)
    expect(result[9].clientId).toBe(7)
})
