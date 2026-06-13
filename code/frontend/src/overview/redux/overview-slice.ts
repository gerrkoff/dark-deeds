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
    isInitialLoadComplete: boolean
    isTasksCacheHydrated: boolean
}

const initialState: OverviewState = {
    tasks: [],
    routineTaskDatesShown: [],
    isLoadTasksPending: false,
    isInitialLoadComplete: false,
    isTasksCacheHydrated: false,
}

export const overviewSlice = createSlice({
    name: 'overview',
    initialState,
    reducers: {
        // local changes
        updateTasks: (state, action: PayloadAction<TaskModel[]>) => {
            for (const task of action.payload) {
                const index = state.tasks.findIndex(t => t.uid === task.uid)
                if (index !== -1) {
                    state.tasks[index] = {
                        ...task,
                        version: state.tasks[index].version,
                    }
                } else {
                    state.tasks.push(task)
                }
            }
        },
        // from online sync
        syncTasks: (state, action: PayloadAction<TasksSyncModel>) => {
            const taskIndexMap = new Map<string, number>(state.tasks.map((x, i) => [x.uid, i]))

            for (const task of action.payload.tasks) {
                const index = taskIndexMap.get(task.uid) ?? -1
                if (index !== -1) {
                    state.tasks[index] = task
                } else {
                    state.tasks.push(task)
                }
            }
        },
        // from a full reload: the snapshot is authoritative, so tasks that are neither in the
        // snapshot nor pending locally (keepUids) are stale (e.g. deleted on another client
        // while offline) and get removed, then the snapshot is merged in.
        reconcileTasks: (state, action: PayloadAction<{ tasks: TaskModel[]; keepUids: string[] }>) => {
            const keep = new Set(action.payload.keepUids)
            state.tasks = state.tasks.filter(task => keep.has(task.uid))

            const taskIndexMap = new Map<string, number>(state.tasks.map((x, i) => [x.uid, i]))

            for (const task of action.payload.tasks) {
                const index = taskIndexMap.get(task.uid) ?? -1
                if (index !== -1) {
                    state.tasks[index] = task
                } else {
                    state.tasks.push(task)
                    taskIndexMap.set(task.uid, state.tasks.length - 1)
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
        // from the offline cache on startup: show the last known tasks immediately, before the
        // server reload reconciles them. This is the moment data first appears (from cache or an
        // empty cache), so it marks the initial load complete - the reload always happens later.
        hydrateTasks: (state, action: PayloadAction<TaskModel[]>) => {
            state.tasks = action.payload
            state.isTasksCacheHydrated = true
            state.isInitialLoadComplete = true
        },
        cleanup: state => {
            state.tasks = []
            state.isTasksCacheHydrated = false
            state.isInitialLoadComplete = false
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

export const {
    updateTasks,
    syncTasks,
    reconcileTasks,
    hydrateTasks,
    updateTaskVersions,
    toggleRoutineTaskDate,
    cleanup,
} = overviewSlice.actions

export default overviewSlice.reducer
