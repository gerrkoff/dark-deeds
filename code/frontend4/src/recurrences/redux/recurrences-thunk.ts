import { createAsyncThunk } from '@reduxjs/toolkit'
import { PlannedRecurrenceModel } from '../models/PlannedRecurrenceModel'
import { recurrencesApi } from '../api/RecurrencesApi'

export const loadRecurrences = createAsyncThunk(
    'recurrences/load',
    async (): Promise<PlannedRecurrenceModel[]> => {
        return await recurrencesApi.loadRecurrences()
    },
)
