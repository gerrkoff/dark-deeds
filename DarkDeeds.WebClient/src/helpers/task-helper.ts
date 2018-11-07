import { DayCardModel, OverviewModel, Task } from '../models'
import { DateHelper } from './'

const service = {
    evalModel(tasks: Task[], now: Date): OverviewModel {
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

        if (sourceTasks) {
            sourceTasks.forEach(x => {
                if (x.order > task.order) {
                    x.order--
                    x.updated = true
                }
            })
        }

        if (!siblingTask) {
            task.order = targetTasks.length > 0
                ? targetTasks.reduce((max, p) => p.order > max.order ? p : max, targetTasks[0]).order + 1
                : 1
        } else {
            const newOrder = siblingTask.order
            targetTasks.forEach(x => {
                if (x.order >= newOrder) {
                    x.order++
                    x.updated = true
                }
            })
            task.order = newOrder
        }

        task.dateTime = targetDate === 0 ? null : new Date(targetDate)
        task.updated = true

        return tasks
    },

    createTaskFromText(text: string): Task {
        const task = {
            clientId: 0,
            id: 0,
            order: 0,
            title: text,
            updated: false
        }

        if (/^\d{4}\s\d{4}/.test(text)) {
            task.title = text.substr(9).trim()
            const month = Number(text.substr(0, 2))
            const day = Number(text.substr(2, 2))
            const currentYear = new Date().getFullYear()
            const hour = Number(text.substr(5, 2))
            const minute = Number(text.substr(7, 2))

            return {
                ...task,
                dateTime: new Date(currentYear, month - 1, day, hour, minute)
            }
        }

        if (/^\d{4}/.test(text)) {
            task.title = text.substr(4).trim()
            const month = Number(text.substr(0, 2))
            const day = Number(text.substr(2, 2))
            const currentYear = new Date().getFullYear()

            return {
                ...task,
                dateTime: new Date(currentYear, month - 1, day)
            }
        }

        return {
            ...task,
            dateTime: null
        }
    }
}

function taskDateToStart(date: Date | null): number {
    if (date) {
        return DateHelper.dayStart(date).getTime()
    } else {
        return 0
    }
}

export { service as TaskHelper }
