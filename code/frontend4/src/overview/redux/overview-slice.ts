import { createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'
import { TaskModel } from '../../tasks/models/TaskModel'
import { loadOverviewTasks } from './overview-thunk'

export interface OverviewState {
    tasks: TaskModel[]
    isLoadTasksPending: boolean
}

const initialState: OverviewState = {
    tasks: [],
    isLoadTasksPending: false,
}

export const overviewSlice = createSlice({
    name: 'overview',
    initialState,
    reducers: {
        updateTasks: (state, action: PayloadAction<TaskModel[]>) => {
            state.tasks.push(...action.payload)
        },
    },
    extraReducers: builder => {
        builder.addCase(loadOverviewTasks.pending, state => {
            state.isLoadTasksPending = true
        })
        builder.addCase(loadOverviewTasks.rejected, state => {
            state.isLoadTasksPending = false
        })
        builder.addCase(loadOverviewTasks.fulfilled, (state, action) => {
            state.isLoadTasksPending = false
            state.tasks = action.payload
        })
    },
})

export const { updateTasks } = overviewSlice.actions

export default overviewSlice.reducer
