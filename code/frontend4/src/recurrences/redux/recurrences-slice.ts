import { createSlice } from '@reduxjs/toolkit'
import { PlannedRecurrenceModel } from '../models/PlannedRecurrenceModel'
import { loadRecurrences } from './recurrences-thunk'

export interface RecurrencesState {
    isLoadPending: boolean
    recurrences: PlannedRecurrenceModel[]
}

const initialState: RecurrencesState = {
    isLoadPending: false,
    recurrences: [],
}

export const recurrencesSlice = createSlice({
    name: 'recurrences',
    initialState,
    reducers: {},
    extraReducers: builder => {
        builder.addCase(loadRecurrences.pending, state => {
            state.isLoadPending = true
            state.recurrences = []
        })
        builder.addCase(loadRecurrences.rejected, state => {
            state.isLoadPending = false
        })
        builder.addCase(loadRecurrences.fulfilled, (state, action) => {
            state.isLoadPending = false
            state.recurrences = action.payload
        })
    },
})

// export const {} = recurrencesSlice.actions

export default recurrencesSlice.reducer
