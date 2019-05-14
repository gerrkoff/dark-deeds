// import { TaskHelper } from '../../helpers'
// import { Task, TaskTimeTypeEnum } from '../../models'

// function task(year: number, month: number, date: number, id: number = 0, order: number = 0, timeType: TaskTimeTypeEnum = TaskTimeTypeEnum.NoTime, hours: number = 0, minutes: number = 0): Task {
//     return new Task(id, '', new Date(year, month, date, hours, minutes), order, false, 0, false, false, timeType)
// }

// test('positive', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 9, 1, 1),
//         task(2018, 9, 9, 2, 2),
//         task(2018, 9, 9, 3, 3),
//         task(2018, 9, 10, 4, 1),
//         task(2018, 9, 10, 5, 2)
//     ]

//     const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), 2)

//     expect(result.find(x => x.clientId === 1)!.order).toBe(1)
//     expect(result.find(x => x.clientId === 2)!.order).toBe(3)
//     expect(result.find(x => x.clientId === 3)!.order).toBe(4)
//     expect(result.find(x => x.clientId === 4)!.order).toBe(2)
//     expect(result.find(x => x.clientId === 5)!.order).toBe(1)

//     expect(result.find(x => x.clientId === 4)!.dateTime!.getTime()).toBe(new Date(2018, 9, 9).getTime())
// })

// test('move as last', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 9, 1, 10),
//         task(2018, 9, 9, 2, 2),
//         task(2018, 9, 10, 3, 1)
//     ]

//     const result = TaskHelper.moveTask(tasks, 3, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), null)

//     expect(result.find(x => x.clientId === 1)!.order).toBe(10)
//     expect(result.find(x => x.clientId === 2)!.order).toBe(2)
//     expect(result.find(x => x.clientId === 3)!.order).toBe(11)
// })

// test('same list reorder', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 9, 1, 4),
//         task(2018, 9, 9, 2, 3),
//         task(2018, 9, 9, 3, 2),
//         task(2018, 9, 9, 4, 1)
//     ]

//     const result = TaskHelper.moveTask(tasks, 2, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 9).getTime(), 4)

//     expect(result.find(x => x.clientId === 1)!.order).toBe(4)
//     expect(result.find(x => x.clientId === 2)!.order).toBe(1)
//     expect(result.find(x => x.clientId === 3)!.order).toBe(3)
//     expect(result.find(x => x.clientId === 4)!.order).toBe(2)
// })

// test('same list as last', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 9, 1, 4),
//         task(2018, 9, 9, 2, 3),
//         task(2018, 9, 9, 3, 2),
//         task(2018, 9, 9, 4, 1)
//     ]

//     const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 9).getTime(), null)

//     expect(result.find(x => x.clientId === 1)!.order).toBe(3)
//     expect(result.find(x => x.clientId === 2)!.order).toBe(2)
//     expect(result.find(x => x.clientId === 3)!.order).toBe(1)
//     expect(result.find(x => x.clientId === 4)!.order).toBe(4)
// })

// test('move concrete time', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 9, 1, 1),
//         task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 11),
//         task(2018, 9, 9, 3, 2),
//         task(2018, 9, 10, 4, 0, TaskTimeTypeEnum.ConcreteTime, 12),
//         task(2018, 9, 10, 5, 1)
//     ]

//     const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), 1)

//     expect(result.find(x => x.clientId === 1)!.order).toBe(1)
//     expect(result.find(x => x.clientId === 2)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 3)!.order).toBe(2)
//     expect(result.find(x => x.clientId === 4)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 5)!.order).toBe(1)

//     expect(result.find(x => x.clientId === 4)!.dateTime!.getTime()).toBe(new Date(2018, 9, 9, 12).getTime())
// })

// test('move after time - from after time to after time', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 9, 1, 1),
//         task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 12, 0),
//         task(2018, 9, 9, 3, 2),
//         task(2018, 9, 10, 4, 0, TaskTimeTypeEnum.AfterTime, 14, 5),
//         task(2018, 9, 10, 5, 1)
//     ]

//     const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), null)

//     expect(result.find(x => x.clientId === 1)!.order).toBe(1)
//     expect(result.find(x => x.clientId === 2)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 3)!.order).toBe(2)
//     expect(result.find(x => x.clientId === 5)!.order).toBe(1)

//     const movedTask = result.find(x => x.clientId === 4)!
//     expect(movedTask.order).toBe(1)
//     expect(movedTask.dateTime!.getTime()).toBe(new Date(2018, 9, 9, 12).getTime())
//     expect(movedTask.timeType).toBe(TaskTimeTypeEnum.AfterTime)
// })

// test('move after time - from no time to after time', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 9, 1, 1),
//         task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 12),
//         task(2018, 9, 9, 3, 0, TaskTimeTypeEnum.ConcreteTime, 13),
//         task(2018, 9, 10, 4, 1),
//         task(2018, 9, 10, 5, 2)
//     ]

//     const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), 3)

//     expect(result.find(x => x.clientId === 1)!.order).toBe(1)
//     expect(result.find(x => x.clientId === 2)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 3)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 5)!.order).toBe(1)

//     const movedTask = result.find(x => x.clientId === 4)!
//     expect(movedTask.order).toBe(1)
//     expect(movedTask.dateTime!.getTime()).toBe(new Date(2018, 9, 9, 12).getTime())
//     expect(movedTask.timeType).toBe(TaskTimeTypeEnum.AfterTime)
// })

// test('move after time - from after time to no time', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 9, 1, 1),
//         task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 12),
//         task(2018, 9, 10, 3, 0, TaskTimeTypeEnum.ConcreteTime, 13),
//         task(2018, 9, 10, 4, 1, TaskTimeTypeEnum.AfterTime, 13),
//         task(2018, 9, 10, 5, 1)
//     ]

//     const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), 2)

//     expect(result.find(x => x.clientId === 1)!.order).toBe(1)
//     expect(result.find(x => x.clientId === 2)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 3)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 5)!.order).toBe(1)

//     const movedTask = result.find(x => x.clientId === 4)!
//     expect(movedTask.order).toBe(2)
//     expect(movedTask.dateTime!.getTime()).toBe(new Date(2018, 9, 9).getTime())
//     expect(movedTask.timeType).toBe(TaskTimeTypeEnum.NoTime)
// })

// test('moving to no date (reset type)', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 9, 1, 1),
//         task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 12),
//         task(2018, 9, 10, 3, 0, TaskTimeTypeEnum.ConcreteTime, 13),
//         task(2018, 9, 10, 4, 1, TaskTimeTypeEnum.AfterTime, 13),
//         task(2018, 9, 10, 5, 2, TaskTimeTypeEnum.AfterTime, 13)
//     ]

//     const result = TaskHelper.moveTask(tasks, 4, 0, new Date(2018, 9, 10).getTime(), null)

//     expect(result.find(x => x.clientId === 1)!.order).toBe(1)
//     expect(result.find(x => x.clientId === 2)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 3)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 5)!.order).toBe(1)

//     const movedTask = result.find(x => x.clientId === 4)!
//     expect(movedTask.order).toBe(1)
//     expect(movedTask.dateTime).toBeNull()
//     expect(movedTask.timeType).toBe(TaskTimeTypeEnum.NoTime)
// })

// test('move concrete time and adjust source aftertime to be no time', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 9, 1, 1),
//         task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 11),
//         task(2018, 9, 9, 3, 2),
//         task(2018, 9, 10, 4, 0, TaskTimeTypeEnum.ConcreteTime, 12),
//         task(2018, 9, 10, 5, 1),
//         task(2018, 9, 10, 6, 1, TaskTimeTypeEnum.AfterTime, 12),
//         task(2018, 9, 10, 7, 2, TaskTimeTypeEnum.AfterTime, 12)
//     ]

//     const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), 1)

//     expect(result.find(x => x.clientId === 1)!.order).toBe(1)
//     expect(result.find(x => x.clientId === 2)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 3)!.order).toBe(2)
//     expect(result.find(x => x.clientId === 4)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 5)!.order).toBe(1)

//     const taskRes1 = result.find(x => x.clientId === 6)!
//     expect(taskRes1.order).toBe(2)
//     expect(taskRes1.dateTime!.getTime()).toBe(new Date(2018, 9, 10).getTime())
//     expect(taskRes1.timeType).toBe(TaskTimeTypeEnum.NoTime)

//     const taskRes2 = result.find(x => x.clientId === 7)!
//     expect(taskRes2.order).toBe(3)
//     expect(taskRes2.dateTime!.getTime()).toBe(new Date(2018, 9, 10).getTime())
//     expect(taskRes2.timeType).toBe(TaskTimeTypeEnum.NoTime)
// })

// test('move concrete time and adjust source aftertime to be after time (concrete time)', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 9, 1, 1),
//         task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 11),
//         task(2018, 9, 9, 3, 2),
//         task(2018, 9, 10, 4, 0, TaskTimeTypeEnum.ConcreteTime, 12),
//         task(2018, 9, 10, 5, 0, TaskTimeTypeEnum.ConcreteTime, 11),
//         task(2018, 9, 10, 6, 1, TaskTimeTypeEnum.AfterTime, 12),
//         task(2018, 9, 10, 7, 2, TaskTimeTypeEnum.AfterTime, 12)
//     ]

//     const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), 1)

//     expect(result.find(x => x.clientId === 1)!.order).toBe(1)
//     expect(result.find(x => x.clientId === 2)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 3)!.order).toBe(2)
//     expect(result.find(x => x.clientId === 4)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 5)!.order).toBe(0)

//     const taskRes1 = result.find(x => x.clientId === 6)!
//     expect(taskRes1.order).toBe(1)
//     expect(taskRes1.dateTime!.getTime()).toBe(new Date(2018, 9, 10, 11).getTime())
//     expect(taskRes1.timeType).toBe(TaskTimeTypeEnum.AfterTime)

//     const taskRes2 = result.find(x => x.clientId === 7)!
//     expect(taskRes2.order).toBe(2)
//     expect(taskRes2.dateTime!.getTime()).toBe(new Date(2018, 9, 10, 11).getTime())
//     expect(taskRes2.timeType).toBe(TaskTimeTypeEnum.AfterTime)
// })

// test('move concrete time and adjust source aftertime to be after time (after time)', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 9, 1, 1),
//         task(2018, 9, 9, 2, 0, TaskTimeTypeEnum.ConcreteTime, 11),
//         task(2018, 9, 9, 3, 2),
//         task(2018, 9, 10, 4, 0, TaskTimeTypeEnum.ConcreteTime, 12),
//         task(2018, 9, 10, 5, 0, TaskTimeTypeEnum.ConcreteTime, 11),
//         task(2018, 9, 10, 6, 1, TaskTimeTypeEnum.AfterTime, 12),
//         task(2018, 9, 10, 7, 2, TaskTimeTypeEnum.AfterTime, 12),
//         task(2018, 9, 10, 8, 1, TaskTimeTypeEnum.AfterTime, 11)
//     ]

//     const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 9).getTime(), new Date(2018, 9, 10).getTime(), 1)

//     expect(result.find(x => x.clientId === 1)!.order).toBe(1)
//     expect(result.find(x => x.clientId === 2)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 3)!.order).toBe(2)
//     expect(result.find(x => x.clientId === 4)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 5)!.order).toBe(0)
//     expect(result.find(x => x.clientId === 8)!.order).toBe(1)

//     const taskRes1 = result.find(x => x.clientId === 6)!
//     expect(taskRes1.order).toBe(2)
//     expect(taskRes1.dateTime!.getTime()).toBe(new Date(2018, 9, 10, 11).getTime())
//     expect(taskRes1.timeType).toBe(TaskTimeTypeEnum.AfterTime)

//     const taskRes2 = result.find(x => x.clientId === 7)!
//     expect(taskRes2.order).toBe(3)
//     expect(taskRes2.dateTime!.getTime()).toBe(new Date(2018, 9, 10, 11).getTime())
//     expect(taskRes2.timeType).toBe(TaskTimeTypeEnum.AfterTime)
// })

// test('move concrete time and no adjust if source and target are the same', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 10, 4, 0, TaskTimeTypeEnum.ConcreteTime, 12),
//         task(2018, 9, 10, 8, 1, TaskTimeTypeEnum.AfterTime, 12)
//     ]

//     const result = TaskHelper.moveTask(tasks, 4, new Date(2018, 9, 10).getTime(), new Date(2018, 9, 10).getTime(), null)

//     expect(result.find(x => x.clientId === 4)!.order).toBe(0)

//     const taskRes = result.find(x => x.clientId === 8)!
//     expect(taskRes.order).toBe(1)
//     expect(taskRes.dateTime!.getTime()).toBe(new Date(2018, 9, 10, 12).getTime())
//     expect(taskRes.timeType).toBe(TaskTimeTypeEnum.AfterTime)
// })

// test('same list no date 1', () => {
//     const tasks: Task[] = [
//         new Task(1, '', null, 1, false, 0, false, false, TaskTimeTypeEnum.NoTime),
//         new Task(2, '', null, 2, false, 0, false, false, TaskTimeTypeEnum.NoTime),
//         new Task(3, '', null, 4, false, 0, false, false, TaskTimeTypeEnum.NoTime),
//         new Task(4, '', null, 3, false, 0, false, false, TaskTimeTypeEnum.NoTime)
//     ]

//     const result = TaskHelper.moveTask(tasks, 4, 0, 0, 1)

//     expect(result.find(x => x.clientId === 1)!.order).toBe(2)
//     expect(result.find(x => x.clientId === 2)!.order).toBe(3)
//     expect(result.find(x => x.clientId === 3)!.order).toBe(4)
//     expect(result.find(x => x.clientId === 4)!.order).toBe(1)
// })

// test('same list no date 2', () => {
//     const tasks: Task[] = [
//         new Task(1, '', null, 1, false, 0, false, false, TaskTimeTypeEnum.NoTime),
//         new Task(2, '', null, 2, false, 0, false, false, TaskTimeTypeEnum.NoTime),
//         new Task(3, '', null, 4, false, 0, false, false, TaskTimeTypeEnum.NoTime),
//         new Task(4, '', null, 3, false, 0, false, false, TaskTimeTypeEnum.NoTime)
//     ]

//     const result = TaskHelper.moveTask(tasks, 3, 0, 0, 4)

//     expect(result.find(x => x.clientId === 1)!.order).toBe(1)
//     expect(result.find(x => x.clientId === 2)!.order).toBe(2)
//     expect(result.find(x => x.clientId === 3)!.order).toBe(3)
//     expect(result.find(x => x.clientId === 4)!.order).toBe(4)
// })

// test('move to list with only all day long tasks equals moving to empty list - No time should not change', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 10, 1, 0, TaskTimeTypeEnum.AllDayLong),
//         task(2018, 9, 11, 2, 0, TaskTimeTypeEnum.NoTime)
//     ]

//     const result = TaskHelper.moveTask(tasks, 2, new Date(2018, 9, 10).getTime(), new Date(2018, 9, 11).getTime(), null)

//     const taskRes = result.find(x => x.clientId === 2)!
//     expect(taskRes.dateTime!.getTime()).toBe(new Date(2018, 9, 10).getTime())
//     expect(taskRes.timeType).toBe(TaskTimeTypeEnum.NoTime)
// })

// test('move to list with only all day long tasks equals moving to empty list - After time should change to No time', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 10, 1, 0, TaskTimeTypeEnum.AllDayLong),
//         task(2018, 9, 11, 2, 0, TaskTimeTypeEnum.AfterTime, 12)
//     ]

//     const result = TaskHelper.moveTask(tasks, 2, new Date(2018, 9, 10).getTime(), new Date(2018, 9, 11).getTime(), null)

//     const taskRes = result.find(x => x.clientId === 2)!
//     expect(taskRes.dateTime!.getTime()).toBe(new Date(2018, 9, 10).getTime())
//     expect(taskRes.timeType).toBe(TaskTimeTypeEnum.NoTime)
// })

// test('move concrete time task from list with all day long task should reset remaining after time tasks to no time', () => {
//     const tasks: Task[] = [
//         task(2018, 9, 10, 1, 0, TaskTimeTypeEnum.AllDayLong),
//         task(2018, 9, 10, 2, 0, TaskTimeTypeEnum.ConcreteTime, 12),
//         task(2018, 9, 10, 3, 0, TaskTimeTypeEnum.AfterTime, 12)
//     ]

//     const result = TaskHelper.moveTask(tasks, 2, new Date(2018, 9, 11).getTime(), new Date(2018, 9, 10).getTime(), null)

//     const taskRes = result.find(x => x.clientId === 3)!
//     expect(taskRes.dateTime!.getTime()).toBe(new Date(2018, 9, 10).getTime())
//     expect(taskRes.timeType).toBe(TaskTimeTypeEnum.NoTime)
// })
