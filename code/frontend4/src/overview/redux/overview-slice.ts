import { createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'
import { TaskModel } from '../../tasks/models/TaskModel'
import { loadOverviewTasks } from './overview-thunk'
import { TaskVersionModel } from '../../tasks/models/TaskVersionModel'

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
            state.tasks = [
                ...state.tasks.filter(
                    task => !action.payload.find(t => t.uid === task.uid),
                ),
                ...action.payload,
            ]
        },
        updateVersions: (state, action: PayloadAction<TaskVersionModel[]>) => {
            state.tasks.forEach(task => {
                const version = action.payload.find(x => x.uid === task.uid)
                if (version) {
                    task.version = version.version
                }
            })
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

export const { updateTasks, updateVersions } = overviewSlice.actions

export default overviewSlice.reducer
