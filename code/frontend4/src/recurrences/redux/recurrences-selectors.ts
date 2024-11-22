import { createSelector } from '@reduxjs/toolkit'
import { RootState } from '../../store'

export const recurrencesSelector = createSelector(
    (state: RootState) => state.recurrences.recurrences,
    recurrences => recurrences.filter(r => !r.isDeleted),
)
