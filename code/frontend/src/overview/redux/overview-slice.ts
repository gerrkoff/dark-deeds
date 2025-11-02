import { createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'
import { TaskModel } from '../../tasks/models/TaskModel'
import { reloadOverviewTasks } from './overview-thunk'
import { TasksSyncModel } from '../../tasks/models/TasksSyncModel'
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
                    if (task.version < state.tasks[index].version) {
                        console.warn(
                            'Update Tasks Collision',
                            {
                                existing: state.tasks[index].version,
                                incoming: task.version,
                            },
                            {
                                existing: { ...state.tasks[index] },
                                incoming: { ...task },
                            },
                        )
                    }

                    state.tasks[index] = {
                        ...task,
                        version: state.tasks[index].version,
                    }
                } else {
                    state.tasks.push(task)
                }
            }
        },
        syncTasks: (state, action: PayloadAction<TasksSyncModel>) => {
            const taskIndexMap = new Map<string, number>(state.tasks.map((x, i) => [x.uid, i]))

            for (const task of action.payload.tasks) {
                const index = taskIndexMap.get(task.uid) ?? -1
                if (index !== -1) {
                    if (task.version < state.tasks[index].version) {
                        console.warn(
                            'Sync Tasks Collision',
                            {
                                existing: state.tasks[index].version,
                                incoming: task.version,
                            },
                            {
                                existing: { ...state.tasks[index] },
                                incoming: { ...task },
                            },
                        )
                    }
                    state.tasks[index] = task
                } else {
                    state.tasks.push(task)
                }
            }
        },
        updateTaskVersions: (state, action: PayloadAction<TaskVersionModel[]>) => {
            const taskIndexMap = new Map<string, number>(state.tasks.map((x, i) => [x.uid, i]))

            for (const task of action.payload) {
                const index = taskIndexMap.get(task.uid) ?? -1
                if (index !== -1) {
                    state.tasks[index].version = task.version
                }
            }
        },
        cleanup: state => {
            state.tasks = []
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

export const { updateTasks, syncTasks, updateTaskVersions, toggleRoutineTaskDate, cleanup } = overviewSlice.actions

export default overviewSlice.reducer
