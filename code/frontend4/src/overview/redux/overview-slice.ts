import { createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'
import { TaskModel } from '../../tasks/models/TaskModel'
import { loadOverviewTasks, reloadOverviewTasks } from './overview-thunk'
import { TaskVersionModel } from '../../tasks/models/TaskVersionModel'

export interface OverviewState {
    tasks: TaskModel[]
    routineTaskDatesShown: number[]
    isLoadTasksPending: boolean
}

const initialState: OverviewState = {
    tasks: [],
    routineTaskDatesShown: [],
    isLoadTasksPending: false,
}

export const overviewSlice = createSlice({
    name: 'overview',
    initialState,
    reducers: {
        updateTasks: (state, action: PayloadAction<TaskModel[]>) => {
            for (const task of action.payload) {
                const index = state.tasks.findIndex(t => t.uid === task.uid)
                if (index !== -1) {
                    state.tasks[index] = task
                } else {
                    state.tasks.push(task)
                }
            }
        },
        updateVersions: (state, action: PayloadAction<TaskVersionModel[]>) => {
            for (const task of action.payload) {
                const index = state.tasks.findIndex(t => t.uid === task.uid)
                if (index !== -1) {
                    state.tasks[index].version = task.version
                }
            }
        },
        toggleRoutineTaskDate: (state, action: PayloadAction<number>) => {
            const index = state.routineTaskDatesShown.indexOf(action.payload)
            if (index !== -1) {
                state.routineTaskDatesShown.splice(index, 1)
            } else {
                state.routineTaskDatesShown.push(action.payload)
            }
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

        builder.addCase(reloadOverviewTasks.pending, state => {
            state.isLoadTasksPending = true
        })
        builder.addCase(reloadOverviewTasks.rejected, state => {
            state.isLoadTasksPending = false
        })
        builder.addCase(reloadOverviewTasks.fulfilled, state => {
            state.isLoadTasksPending = false
        })
    },
})

export const { updateTasks, updateVersions, toggleRoutineTaskDate } =
    overviewSlice.actions

export default overviewSlice.reducer
