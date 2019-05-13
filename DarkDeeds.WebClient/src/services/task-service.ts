import { DayCardModel, OverviewModel, Task, TaskTimeTypeEnum } from '../models'
import { DateService } from '.'

const service = {
    evalModel(tasks: Task[], now: Date, showCompleted: boolean): OverviewModel {
        tasks = tasks.filter(x =>
            (showCompleted || !x.completed) &&
            !x.deleted)

        const model = new OverviewModel()
        const currentStart = DateService.monday(DateService.dayStart(now))
        const futureStart = new Date(currentStart)
        futureStart.setDate(currentStart.getDate() + 14)

        for (const iDate = new Date(currentStart); iDate < futureStart; iDate.setDate(iDate.getDate() + 1)) {
            const day = new DayCardModel(new Date(iDate))
            model.current.push(day)
        }

        tasks.forEach(task => {
            if (task.dateTime === null) {
                model.noDate.push(task)
                return
            }

            const days: DayCardModel[] = task.dateTime < currentStart
                ? model.expired
                : task.dateTime >= futureStart
                    ? model.future
                    : model.current

            const taskDate = DateService.dayStart(task.dateTime)
            let day = days.find(x => x.date.getTime() === taskDate.getTime())

            if (day === undefined) {
                day = new DayCardModel(taskDate)
                days.push(day)
            }

            day.tasks.push(task)
        })

        return model
    },

    moveTask(tasks: Task[], taskId: number, targetDate: number, sourceDate: number, siblingId: number | null): Task[] {
        const taskIndex = tasks.findIndex(x => x.clientId === taskId)

        if (taskIndex === -1) {
            return tasks
        }

        const oldTask = tasks[taskIndex]
        tasks[taskIndex] = {
            ...tasks[taskIndex],
            changed: true
        }
        const task = tasks[taskIndex]
        let taskBeforeOldData = {
            dateTime: new Date(sourceDate),
            order: 1,
            timeType: TaskTimeTypeEnum.NoTime
        }

        // CHANGE TASK
        if (task.timeType === TaskTimeTypeEnum.ConcreteTime) {
            if (targetDate !== sourceDate) {
                const sourceTasksSorted = this.sortTasks(tasks.filter(x => taskDateToStart(x.dateTime) === sourceDate))
                const taskBeforeOldIndex = sourceTasksSorted.findIndex(x => x.clientId === oldTask.clientId) - 1
                if (taskBeforeOldIndex !== -1) {
                    taskBeforeOldData = {
                        dateTime: new Date(sourceTasksSorted[taskBeforeOldIndex].dateTime!),
                        order: sourceTasksSorted[taskBeforeOldIndex].order + 1,
                        timeType:
                            sourceTasksSorted[taskBeforeOldIndex].timeType === TaskTimeTypeEnum.NoTime ||
                            sourceTasksSorted[taskBeforeOldIndex].timeType === TaskTimeTypeEnum.AllDayLong
                                ? TaskTimeTypeEnum.NoTime
                                : TaskTimeTypeEnum.AfterTime
                    }
                }
            }

            task.dateTime = new Date(targetDate)
            task.dateTime.setHours(oldTask.dateTime!.getHours())
            task.dateTime.setMinutes(oldTask.dateTime!.getMinutes())

        } else {
            const targetTasksSorted = this.sortTasks(tasks.filter(x => taskDateToStart(x.dateTime) === targetDate))
            const previosSiblingIndex = siblingId
                ? targetTasksSorted.findIndex(x => x.clientId === siblingId) - 1
                : targetTasksSorted.length - 1
            const sameGroupAsc = previosSiblingIndex !== -1
                ? tasksInTheSameGroup(oldTask, targetTasksSorted[previosSiblingIndex]) && targetTasksSorted[previosSiblingIndex].order > task.order
                : false

            if (previosSiblingIndex === -1 ||
                targetTasksSorted[previosSiblingIndex].timeType === TaskTimeTypeEnum.NoTime ||
                targetTasksSorted[previosSiblingIndex].timeType === TaskTimeTypeEnum.AllDayLong
            ) {
                task.timeType = TaskTimeTypeEnum.NoTime
                task.dateTime = targetDate === 0 ? null : new Date(targetDate)
                task.order = previosSiblingIndex !== -1
                    ? targetTasksSorted[previosSiblingIndex].order + (sameGroupAsc ? 0 : 1)
                    : 1
            } else {
                task.timeType = TaskTimeTypeEnum.AfterTime
                task.dateTime = new Date(targetTasksSorted[previosSiblingIndex].dateTime!)
                task.order = targetTasksSorted[previosSiblingIndex].timeType === TaskTimeTypeEnum.AfterTime
                    ? targetTasksSorted[previosSiblingIndex].order + (sameGroupAsc ? 0 : 1)
                    : 1
            }
        }

        // ADJUST OTHER TASKS
        for (let i = 0; i < tasks.length; i++) {
            if (tasks[i].clientId === task.clientId) {
                continue
            }

            if (tasks[i].order > oldTask.order && tasksInTheSameGroup(tasks[i], oldTask)) {
                tasks[i] = {
                    ...tasks[i],
                    order: tasks[i].order - 1,
                    changed: true
                }
            }

            if (tasks[i].order >= task.order && tasksInTheSameGroup(tasks[i], task)) {
                tasks[i] = {
                    ...tasks[i],
                    order: tasks[i].order + 1,
                    changed: true
                }
            }

            if (oldTask.timeType === TaskTimeTypeEnum.ConcreteTime
                    && targetDate !== sourceDate
                    && tasks[i].dateTime
                    && tasks[i].dateTime!.getTime() === oldTask.dateTime!.getTime()) {
                tasks[i] = {
                    ...tasks[i],
                    dateTime: new Date(taskBeforeOldData.dateTime),
                    order: taskBeforeOldData.order++,
                    timeType: taskBeforeOldData.timeType,
                    changed: true
                }
            }
        }

        return [...tasks]
    },

    tasksEqual(taskA: Task, taskB: Task): boolean {
        let dateEquals = false
        if (taskA.dateTime === null && taskB.dateTime === null) {
            dateEquals = true
        } else if (taskA.dateTime !== null && taskB.dateTime !== null) {
            dateEquals = taskA.dateTime.getTime() === taskB.dateTime.getTime()
        } else {
            dateEquals = false
        }

        return dateEquals
            && taskA.title === taskB.title
            && taskA.order === taskB.order
            && taskA.id === taskB.id
            && taskA.completed === taskB.completed
            && taskA.deleted === taskB.deleted
            && taskA.timeType === taskB.timeType
            && taskA.isProbable === taskB.isProbable
    },

    sortTasks(tasks: Task[]): Task[] {
        tasks.sort((x, y) => {
            const xOrders = evalOrders(x)
            const yOrders = evalOrders(y)
            for (let i = 0; i < xOrders.length; i++) {
                if (xOrders[i] !== yOrders[i]) {
                    return xOrders[i] > yOrders[i] ? 1 : -1
                }
            }
            return 0
        })
        return tasks
    }
}

function taskDateToStart(date: Date | null): number {
    if (date) {
        return DateService.dayStart(date).getTime()
    } else {
        return 0
    }
}

function evalOrders(task: Task): number[] {
    const orders: number[] = []
    orders.push(task.timeType === TaskTimeTypeEnum.AllDayLong ? 0 : task.timeType === TaskTimeTypeEnum.NoTime ? 1 : 2)
    orders.push(task.dateTime === null ? 0 : task.dateTime.getTime())
    orders.push(task.timeType === TaskTimeTypeEnum.ConcreteTime ? 0 : 1)
    orders.push(task.order)
    return orders
}

function tasksInTheSameGroup(taskA: Task, taskB: Task): boolean {
    return taskA.timeType === taskB.timeType && taskA.timeType === TaskTimeTypeEnum.NoTime && taskDateToStart(taskA.dateTime) === taskDateToStart(taskB.dateTime)
        || taskA.timeType === taskB.timeType && taskA.timeType === TaskTimeTypeEnum.AfterTime && taskA.dateTime!.getTime() === taskB.dateTime!.getTime()
}

export { service as TaskService }
