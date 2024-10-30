import { createAsyncThunk } from '@reduxjs/toolkit'
import { TaskModel } from '../../tasks/models/TaskModel'
import { taskApi } from '../../tasks/api/TaskApi'
import { dateService } from '../../common/services/DateService'
import { AppThunk } from '../../store'
import { taskSaveService } from '../../tasks/services/TaskSaveService'
import { updateTasks } from './overview-slice'
import { taskSyncService } from '../../tasks/services/TaskSyncService'

export const loadOverviewTasks = createAsyncThunk(
    'overview/loadTasks',
    (): Promise<TaskModel[]> => {
        return taskApi.loadTasks(dateService.monday(dateService.today()))
    },
)

export const reloadOverviewTasks = createAsyncThunk(
    'overview/reloadTasks',
    (): Promise<TaskModel[]> => {
        return taskApi.loadTasks(dateService.monday(dateService.today()))
    },
)

export const updateAndSyncTasks =
    (tasks: TaskModel[]): AppThunk =>
    (dispatch, getState) => {
        const state = getState().overview.tasks
        const tasksToSync = taskSaveService.getTasksToSync(state, tasks)

        dispatch(updateTasks(tasksToSync))
        taskSyncService.sync(tasksToSync)
    }
