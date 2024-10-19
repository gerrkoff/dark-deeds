import { createSelector } from '@reduxjs/toolkit'
import { RootState } from '../../store'
import { TaskEntity } from '../../common/models/TaskEntity'
import { OverviewModel } from '../models/OverviewModel'
import { dateService } from '../../common/services/DateService'
import { DayCardModel } from '../../ui/models/DayCardModel'

export const overviewModelSelector = createSelector(
    (state: RootState) => state.overview.tasks,
    state => overviewModelSelectorFn(state),
)

function overviewModelSelectorFn(tasks: TaskEntity[]): OverviewModel {
    const model: OverviewModel = {
        noDate: [],
        expired: [],
        current: [],
        future: [],
    }
    const currentStart = dateService.monday(dateService.today())
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

    tasks.forEach(task => {
        if (task.date === null) {
            model.noDate.push(task)
            return
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
    })

    return model
}
