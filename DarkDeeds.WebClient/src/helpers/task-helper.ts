import { DayCardModel, OverviewModel, Task, TaskModel, TaskTimeTypeEnum } from '../models'
import { DateHelper } from './'

const service = {
    evalModel(tasks: Task[], now: Date, showCompleted: boolean): OverviewModel {
        tasks = tasks.filter(x =>
            (showCompleted || !x.completed) &&
            !x.deleted)

        const model = new OverviewModel()
        const currentStart = DateHelper.monday(DateHelper.dayStart(now))
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

            const taskDate = DateHelper.dayStart(task.dateTime)
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
            updated: true
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
                        timeType: sourceTasksSorted[taskBeforeOldIndex].timeType === TaskTimeTypeEnum.NoTime
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

            if (previosSiblingIndex === -1 || targetTasksSorted[previosSiblingIndex].timeType === TaskTimeTypeEnum.NoTime) {
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
                    updated: true
                }
            }

            if (tasks[i].order >= task.order && tasksInTheSameGroup(tasks[i], task)) {
                tasks[i] = {
                    ...tasks[i],
                    order: tasks[i].order + 1,
                    updated: true
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
                    updated: true
                }
            }
        }

        return [...tasks]
    },

    convertStringToModel(text: string): TaskModel {
        let year: number = new Date().getFullYear()
        let month: number = 0
        let day: number = 1
        let hour: number = 0
        let minute: number = 0
        let timeType: TaskTimeTypeEnum = TaskTimeTypeEnum.NoTime
        let noDate: boolean = true

        if (/^\d{8}\s/.test(text)) {
            year = Number(text.substr(0, 4))
            month = Number(text.substr(4, 2))
            day = Number(text.substr(6, 2))
            noDate = false
            text = text.slice(9)
        } else if (/^\d{4}\s/.test(text)) {
            month = Number(text.substr(0, 2))
            day = Number(text.substr(2, 2))
            noDate = false
            text = text.slice(5)
        }

        if (/^\d{4}\s/.test(text)) {
            hour = Number(text.substr(0, 2))
            minute = Number(text.substr(2, 2))
            timeType = TaskTimeTypeEnum.ConcreteTime
            text = text.slice(5)
        } else if (/^>\d{4}\s/.test(text)) {
            hour = Number(text.substr(1, 2))
            minute = Number(text.substr(3, 2))
            timeType = TaskTimeTypeEnum.AfterTime
            text = text.slice(6)
        }

        return {
            dateTime: noDate ? null : new Date(year, month - 1, day, hour, minute),
            timeType,
            title: text
        }
    },

    convertModelToString(model: TaskModel): string {
        if (model.dateTime === null) {
            return model.title
        }

        let s: string = ''

        if (new Date().getFullYear() !== model.dateTime.getFullYear()) {
            s += `${model.dateTime.getFullYear()}`
        }

        s += `${str2digits(model.dateTime.getMonth() + 1)}${str2digits(model.dateTime.getDate())} `

        if (model.timeType === TaskTimeTypeEnum.NoTime) {
            return `${s}${model.title}`
        }

        if (model.timeType === TaskTimeTypeEnum.AfterTime) {
            s += '>'
        }

        return `${s}${str2digits(model.dateTime.getHours())}${str2digits(model.dateTime.getMinutes())} ${model.title}`
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
        return DateHelper.dayStart(date).getTime()
    } else {
        return 0
    }
}

function str2digits(n: number): string {
    return n < 10 ? '0' + n : n.toString()
}

function evalOrders(task: Task): number[] {
    const orders: number[] = []
    orders.push(task.timeType === TaskTimeTypeEnum.NoTime ? 0 : 1)
    orders.push(task.dateTime === null ? 0 : task.dateTime.getTime())
    orders.push(task.timeType === TaskTimeTypeEnum.ConcreteTime ? 0 : 1)
    orders.push(task.order)
    return orders
}

function tasksInTheSameGroup(taskA: Task, taskB: Task): boolean {
    return taskA.timeType === taskB.timeType && taskA.timeType === TaskTimeTypeEnum.NoTime && taskDateToStart(taskA.dateTime) === taskDateToStart(taskB.dateTime)
        || taskA.timeType === taskB.timeType && taskA.timeType === TaskTimeTypeEnum.AfterTime && taskA.dateTime!.getTime() === taskB.dateTime!.getTime()
}

export { service as TaskHelper }
