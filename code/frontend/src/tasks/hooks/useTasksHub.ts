import { useEffect } from 'react'
import { useAppDispatch } from '../../hooks'
import {
    taskHubConnected,
    taskHubConnecting,
    taskHubDisconnected,
    toggleSaveTaskPending,
} from '../../status-panel/redux/status-panel-slice'
import { taskHubApi } from '../api/TaskHubApi'
import { TaskModel } from '../models/TaskModel'
import { taskSyncService } from '../services/TaskSyncService'
import { useTasksSynchronization } from './useTasksSynchronization'
import { TaskVersionModel } from '../models/TaskVersionModel'

export function useTasksHub() {
    const dispatch = useAppDispatch()

    useEffect(() => {
        taskHubApi.init()
    }, [])

    const { processTasksOnlineUpdate, processTaskSaveFinish, reloadTasks } = useTasksSynchronization()

    useEffect(() => {
        const handleHubClose = () => {
            dispatch(taskHubDisconnected())
        }

        const handleHubReconnecting = () => {
            dispatch(taskHubConnecting())
        }

        const handleHubReconnected = () => {
            dispatch(taskHubConnected())
            reloadTasks()
        }

        const handleUpdateStatus = (isSynchronizing: boolean) => {
            dispatch(toggleSaveTaskPending(isSynchronizing))
        }

        const handleHeartbeat = () => {
            /* Do nothing */
        }

        const handleTasksUpdate = (tasks: TaskModel[]) => {
            processTasksOnlineUpdate(tasks)
        }

        const handleTaskSaveFinish = (notSaved: number, savedTasks: TaskVersionModel[]) => {
            processTaskSaveFinish(notSaved, savedTasks)
        }

        taskHubApi.onClose(handleHubClose)
        taskHubApi.onReconnecting(handleHubReconnecting)
        taskHubApi.onReconnected(handleHubReconnected)
        taskHubApi.onUpdate(handleTasksUpdate)
        taskHubApi.onHeartbeat(handleHeartbeat)
        taskSyncService.subscribeStatusUpdate(handleUpdateStatus)
        taskSyncService.subscribeSaveFinish(handleTaskSaveFinish)

        return () => {
            taskHubApi.offUpdate(handleTasksUpdate)
            taskHubApi.offHeartbeat(handleHeartbeat)
            taskSyncService.unsubscribeStatusUpdate(handleUpdateStatus)
            taskSyncService.unsubscribeSaveFinish(handleTaskSaveFinish)
        }
    }, [dispatch, processTasksOnlineUpdate, processTaskSaveFinish, reloadTasks])
}
