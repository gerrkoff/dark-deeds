import { createAsyncThunk } from '@reduxjs/toolkit'
import { PlannedRecurrenceModel } from '../models/PlannedRecurrenceModel'
import { recurrencesApi } from '../api/RecurrencesApi'
import { dateService } from '../../common/services/DateService'

export const loadRecurrences = createAsyncThunk('recurrences/load', async (): Promise<PlannedRecurrenceModel[]> => {
    return await recurrencesApi.loadRecurrences()
})

export const saveRecurrences = createAsyncThunk(
    'recurrences/save',
    async (recurrences: PlannedRecurrenceModel[]): Promise<number> => {
        return await recurrencesApi.saveRecurrences(recurrences)
    },
)

export const createRecurrences = createAsyncThunk('recurrences/create', async (): Promise<number> => {
    return await recurrencesApi.createRecurrences(dateService.getTimezoneOffset())
})
