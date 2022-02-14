import { DayCardModel, OverviewModel, Task } from '../../models'
import { DateService, dateService } from './date-service'

export class TaskService {
    public constructor(private dateService: DateService) {}

    public evalModel(tasks: Task[], showCompleted: boolean): OverviewModel {
        tasks = tasks.filter(x => (showCompleted || !x.completed) && !x.deleted)

        const model = new OverviewModel()
        const currentStart = this.dateService.monday(this.dateService.today())
        const futureStart = new Date(currentStart)
        futureStart.setDate(currentStart.getDate() + 14)

        for (
            const iDate = new Date(currentStart);
            iDate < futureStart;
            iDate.setDate(iDate.getDate() + 1)
        ) {
            const day = new DayCardModel(new Date(iDate))
            model.current.push(day)
        }

        tasks.forEach(task => {
            if (task.date === null) {
                model.noDate.push(task)
                return
            }

            const days: DayCardModel[] =
                task.date < currentStart
                    ? model.expired
                    : task.date >= futureStart
                    ? model.future
                    : model.current

            let day = days.find(x => x.date.getTime() === task.date!.getTime())

            if (day === undefined) {
                day = new DayCardModel(task.date)
                days.push(day)
            }

            day.tasks.push(task)
        })

        return model
    }

    public sorting(taskA: Task, taskB: Task): number {
        return taskA.order > taskB.order ? 1 : -1
    }
}

export const taskService = new TaskService(dateService)
