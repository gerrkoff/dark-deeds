import { createAsyncThunk } from '@reduxjs/toolkit'
import { TaskModel } from '../../tasks/models/TaskModel'
import { taskApi } from '../../tasks/api/TaskApi'
import { dateService } from '../../common/services/DateService'

export const loadOverviewTasks = createAsyncThunk(
    'overview/loadTasks',
    (): Promise<TaskModel[]> => {
        return taskApi.loadTasks(dateService.monday(dateService.today()))
    },
)
