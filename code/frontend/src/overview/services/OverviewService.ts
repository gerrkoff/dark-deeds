import { dateService, DateService } from '../../common/services/DateService'
import { DayCardModel } from '../../day-card/models/DayCardModel'
import { TaskModel } from '../../tasks/models/TaskModel'
import { OverviewModel } from '../models/OverviewModel'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'

export class OverviewService {
    constructor(private dateService: DateService) {}

    getModel(tasks: TaskModel[], isCompletedShown: boolean): OverviewModel {
        const model: OverviewModel = {
            noDate: [],
            weekly: [],
            expired: [],
            current: [],
            future: [],
        }
        const currentStart = this.dateService.monday(this.dateService.today())
        const futureStart = new Date(currentStart)
        futureStart.setDate(currentStart.getDate() + 14)

        for (
            const iDate = new Date(currentStart);
            iDate < futureStart;
            iDate.setDate(iDate.getDate() + 1)
        ) {
            const day: DayCardModel = {
                date: new Date(iDate),
                tasks: [],
            }
            model.current.push(day)
        }

        for (const task of tasks) {
            if (task.deleted || (!isCompletedShown && task.completed)) {
                continue
            }

            if (task.type === TaskTypeEnum.Weekly) {
                model.weekly.push(task)
                continue
            }

            if (task.date === null) {
                model.noDate.push(task)
                continue
            }

            const taskDate = new Date(task.date)
            const days: DayCardModel[] =
                taskDate < currentStart
                    ? model.expired
                    : taskDate >= futureStart
                      ? model.future
                      : model.current

            let day = days.find(x => x.date.valueOf() === task.date)

            if (day === undefined) {
                day = {
                    date: taskDate,
                    tasks: [],
                }
                days.push(day)
            }

            day.tasks.push(task)
        }

        model.noDate.sort((a, b) => a.order - b.order)
        model.weekly.sort((a, b) => a.order - b.order)
        model.expired.forEach(day =>
            day.tasks.sort((a, b) => a.order - b.order),
        )
        model.current.forEach(day =>
            day.tasks.sort((a, b) => a.order - b.order),
        )
        model.future.forEach(day => day.tasks.sort((a, b) => a.order - b.order))

        model.expired.sort((a, b) => a.date.valueOf() - b.date.valueOf())
        model.future.sort((a, b) => a.date.valueOf() - b.date.valueOf())

        return model
    }
}

export const overviewService = new OverviewService(dateService)
