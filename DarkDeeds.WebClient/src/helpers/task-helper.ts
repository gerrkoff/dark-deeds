import { DayCardModel, OverviewModel, Task, TaskModel, TaskTimeTypeEnum } from '../models'
import { DateHelper } from './'

const service = {
    evalModel(tasks: Task[], now: Date): OverviewModel {
        tasks = tasks.filter(x => !x.deleted)

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

    // TODO: adjust for after time
    moveTask(tasks: Task[], taskId: number, targetDate: number, sourceDate: number, siblingId: number | null): Task[] {
        const task = tasks.find(x => x.clientId === taskId)

        if (task === undefined) {
            return tasks
        }

        const targetTasks = tasks.filter(x => taskDateToStart(x.dateTime) === targetDate)
        const sourceTasks = targetDate !== sourceDate
            ? tasks.filter(x => taskDateToStart(x.dateTime) === sourceDate)
            : targetTasks
        const siblingTask = siblingId && targetTasks
            ? targetTasks.find(x => x.clientId === siblingId)
            : null

        if (task.timeType === TaskTimeTypeEnum.NoTime) {
            if (sourceTasks) {
                sourceTasks.forEach(x => {
                    if (x.order > task.order && x.timeType === TaskTimeTypeEnum.NoTime) {
                        x.order--
                        x.updated = true
                    }
                })
            }
            if (!siblingTask) {
                task.order = targetTasks.length > 0
                    ? targetTasks.filter(x => x.timeType === TaskTimeTypeEnum.NoTime).reduce((max, p) => p.order > max.order ? p : max, targetTasks[0]).order + 1
                    : 1
            } else {
                const newOrder = siblingTask.order
                targetTasks.forEach(x => {
                    if (x.order >= newOrder && x.timeType === TaskTimeTypeEnum.NoTime) {
                        x.order++
                        x.updated = true
                    }
                })
                task.order = newOrder
            }
        }

        const targetDateAsDate = new Date(targetDate)
        task.dateTime = targetDate === 0 ? null : new Date(targetDateAsDate.getFullYear(), targetDateAsDate.getMonth(), targetDateAsDate.getDate(), task.dateTime!.getHours(), task.dateTime!.getMinutes())
        task.updated = true

        return tasks
    },

    // TODO: implement future year support
    convertStringToModel(text: string): TaskModel {
        const model = new TaskModel(text)

        if (/^\d{4}\s\d{4}/.test(text)) {
            model.title = text.substr(9).trim()
            const month = Number(text.substr(0, 2))
            const day = Number(text.substr(2, 2))
            const currentYear = new Date().getFullYear()
            const hour = Number(text.substr(5, 2))
            const minute = Number(text.substr(7, 2))

            return {
                ...model,
                dateTime: new Date(currentYear, month - 1, day, hour, minute),
                timeType: TaskTimeTypeEnum.ConcreteTime
            }
        }

        if (/^\d{4}/.test(text)) {
            model.title = text.substr(4).trim()
            const month = Number(text.substr(0, 2))
            const day = Number(text.substr(2, 2))
            const currentYear = new Date().getFullYear()

            return {
                ...model,
                dateTime: new Date(currentYear, month - 1, day),
                timeType: TaskTimeTypeEnum.NoTime
            }
        }

        return {
            ...model,
            dateTime: null
        }
    },

    // TODO: implement future year support
    convertModelToString(model: TaskModel): string {
        if (model.dateTime === null) {
            return model.title
        }

        const s = `${str2digits(model.dateTime.getMonth() + 1)}${str2digits(model.dateTime.getDate())}`

        if (model.timeType === TaskTimeTypeEnum.NoTime) {
            return `${s} ${model.title}`
        }

        return `${s} ${str2digits(model.dateTime.getHours())}${str2digits(model.dateTime.getMinutes())} ${model.title}`
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

    // TODO: adjust for aftertime
    sortTasks(tasks: Task[]): Task[] {
        tasks.sort((x, y) => {
            if (x.timeType === TaskTimeTypeEnum.ConcreteTime && y.timeType === TaskTimeTypeEnum.NoTime) {
                return 1
            }

            if (x.timeType === TaskTimeTypeEnum.NoTime && y.timeType === TaskTimeTypeEnum.ConcreteTime) {
                return 0
            }

            if (x.timeType === TaskTimeTypeEnum.NoTime && y.timeType === TaskTimeTypeEnum.NoTime) {
                return x.order > y.order ? 1 : 0
            }

            if (x.timeType === TaskTimeTypeEnum.ConcreteTime && y.timeType === TaskTimeTypeEnum.ConcreteTime) {
                return x.dateTime! > y.dateTime! ? 1 : 0
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

export { service as TaskHelper }
