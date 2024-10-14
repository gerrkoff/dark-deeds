import { createSelector } from '@reduxjs/toolkit'
import { RootState } from '../../store'

export const appSelector = createSelector(
    (state: RootState) => state.app,
    state => state,
)
