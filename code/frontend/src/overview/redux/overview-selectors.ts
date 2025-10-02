import { createSelector } from '@reduxjs/toolkit'
import { RootState } from '../../store'
import { overviewService } from '../services/OverviewService'

export const overviewTaskRoutinesSelector = createSelector(
    (state: RootState) => state.overview.routineTaskDatesShown,
    routineTaskDatesShown => new Set(routineTaskDatesShown),
)

export const overviewModelSelector = createSelector(
    [(state: RootState) => state.overview.tasks, (state: RootState) => state.settings.isCompletedShown],
    (tasks, isCompletedShow) => overviewService.getModel(tasks, isCompletedShow),
)
