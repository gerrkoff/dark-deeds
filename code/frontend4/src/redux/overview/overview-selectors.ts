import { createSelector } from '@reduxjs/toolkit'
import { RootState } from '../../store'

export const overviewSelector = createSelector(
    (state: RootState) => state.overview,
    state => state.value,
)
