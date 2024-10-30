import { createSelector } from '@reduxjs/toolkit'
import { RootState } from '../../store'
import { OverviewModel } from '../models/OverviewModel'
import { dateService } from '../../common/services/DateService'
import { DayCardModel } from '../../day-card/models/DayCardModel'
import { TaskModel } from '../../tasks/models/TaskModel'

export const overviewTaskRoutinesSelector = createSelector(
    (state: RootState) => state.overview.routineTaskDatesShown,
    routineTaskDatesShown => new Set(routineTaskDatesShown),
)

export const overviewModelSelector = createSelector(
    [
        (state: RootState) => state.overview.tasks,
        (state: RootState) => state.settings.isCompletedShown,
    ],
    (tasks, isCompletedShow) => overviewModelSelectorFn(tasks, isCompletedShow),
)

function overviewModelSelectorFn(
    tasks: TaskModel[],
    isCompletedShown: boolean,
): OverviewModel {
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

    for (const task of tasks) {
        if (task.deleted || (!isCompletedShown && task.completed)) {
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
    model.current.forEach(day => day.tasks.sort((a, b) => a.order - b.order))
    model.future.forEach(day => day.tasks.sort((a, b) => a.order - b.order))
    model.expired.forEach(day => day.tasks.sort((a, b) => a.order - b.order))

    return model
}
