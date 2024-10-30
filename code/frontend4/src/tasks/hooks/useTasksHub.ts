import { useCallback, useEffect } from 'react'
import { useAppDispatch } from '../../hooks'
import {
    taskHubConnected,
    taskHubConnecting,
    taskHubDisconnected,
    toggleSaveTaskPending,
} from '../../status-panel/redux/status-panel-slice'
import {
    updateTasks,
    updateVersions,
} from '../../overview/redux/overview-slice'
import { TaskModel } from '../models/TaskModel'
import { taskHubApi } from '../api/TaskHubApi'
import { taskSyncService } from '../services/TaskSyncService'
import { reloadOverviewTasks } from '../../overview/redux/overview-thunk'
import { unwrapResult } from '@reduxjs/toolkit'

export function useTasksHub() {
    const dispatch = useAppDispatch()

    useEffect(() => {
        taskHubApi.init()
    }, [])

    const handleUpdateTasks = useCallback(
        (tasks: TaskModel[]) => {
            const { tasksToNotify, versionsToNotify } =
                taskSyncService.updateTasks(tasks)

            console.log('Tasks Online Update:', {
                tasks,
                tasksToNotify,
                versionsToNotify,
            })

            if (tasksToNotify.length > 0) {
                dispatch(updateTasks(tasksToNotify))
            }

            if (versionsToNotify.length > 0) {
                dispatch(updateVersions(versionsToNotify))
            }
        },
        [dispatch],
    )

    useEffect(() => {
        const handleHubClose = () => {
            dispatch(taskHubDisconnected())
        }

        const handleHubReconnecting = () => {
            dispatch(taskHubConnecting())
        }

        const handleHubReconnected = async (): Promise<void> => {
            dispatch(taskHubConnected())
            const reloadOverviewTasksResult = await dispatch(
                reloadOverviewTasks(),
            )
            const tasks = unwrapResult(reloadOverviewTasksResult)
            handleUpdateTasks(tasks)
        }

        const handleUpdateStatus = (isSynchronizing: boolean) => {
            dispatch(toggleSaveTaskPending(isSynchronizing))
        }

        taskHubApi.onClose(handleHubClose)
        taskHubApi.onReconnecting(handleHubReconnecting)
        taskHubApi.onReconnected(handleHubReconnected)
        taskHubApi.onUpdate(handleUpdateTasks)
        taskSyncService.subscribeStatusUpdate(handleUpdateStatus)

        return () => {
            taskHubApi.offUpdate(handleUpdateTasks)
            taskSyncService.unsubscribeStatusUpdate(handleUpdateStatus)
        }
    }, [dispatch, handleUpdateTasks])
}