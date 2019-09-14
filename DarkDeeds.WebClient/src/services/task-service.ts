import { DayCardModel, OverviewModel, Task } from '../models'
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
            if (task.date === null) {
                model.noDate.push(task)
                return
            }

            const days: DayCardModel[] = task.date < currentStart
                ? model.expired
                : task.date >= futureStart
                    ? model.future
                    : model.current

            const taskDate = DateService.dayStart(task.date)
            let day = days.find(x => x.date.getTime() === taskDate.getTime())

            if (day === undefined) {
                day = new DayCardModel(taskDate)
                days.push(day)
            }

            day.tasks.push(task)
        })

        return model
    },

    tasksEqual(taskA: Task, taskB: Task): boolean {
        let dateEquals = false
        if (taskA.date === null && taskB.date === null) {
            dateEquals = true
        } else if (taskA.date !== null && taskB.date !== null) {
            dateEquals = taskA.date.getTime() === taskB.date.getTime()
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
            && taskA.time === taskB.time
    },

    sorting(taskA: Task, taskB: Task): number {
        return taskA.order === taskB.order
            ? taskA.clientId > taskB.clientId ? 1 : -1
            : taskA.order > taskB.order ? 1 : -1
    }
}

export { service as TaskService }
