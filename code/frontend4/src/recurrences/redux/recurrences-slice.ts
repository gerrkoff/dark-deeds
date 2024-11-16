import { createSlice } from '@reduxjs/toolkit'
import { PlannedRecurrenceModel } from '../models/PlannedRecurrenceModel'
import {
    createRecurrences,
    loadRecurrences,
    saveRecurrences,
} from './recurrences-thunk'

export interface RecurrencesState {
    recurrences: PlannedRecurrenceModel[]
    isLoadPending: boolean
    isSavePending: boolean
    isCreatePending: boolean
}

const initialState: RecurrencesState = {
    recurrences: [],
    isLoadPending: false,
    isSavePending: false,
    isCreatePending: false,
}

export const recurrencesSlice = createSlice({
    name: 'recurrences',
    initialState,
    reducers: {},
    extraReducers: builder => {
        builder.addCase(loadRecurrences.pending, state => {
            state.isLoadPending = true
        })
        builder.addCase(loadRecurrences.rejected, state => {
            state.isLoadPending = false
        })
        builder.addCase(loadRecurrences.fulfilled, (state, action) => {
            state.isLoadPending = false
            state.recurrences = action.payload
        })

        builder.addCase(saveRecurrences.pending, state => {
            state.isSavePending = true
        })
        builder.addCase(saveRecurrences.rejected, state => {
            state.isSavePending = false
        })
        builder.addCase(saveRecurrences.fulfilled, state => {
            state.isSavePending = false
        })

        builder.addCase(createRecurrences.pending, state => {
            state.isCreatePending = true
        })
        builder.addCase(createRecurrences.rejected, state => {
            state.isCreatePending = false
        })
        builder.addCase(createRecurrences.fulfilled, state => {
            state.isCreatePending = false
        })
    },
})

// export const {} = recurrencesSlice.actions

export default recurrencesSlice.reducer
