import { useEffect } from 'react'
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

export function useTasksHub() {
    const dispatch = useAppDispatch()

    useEffect(() => {
        taskHubApi.init()
    }, [])

    useEffect(() => {
        const handleHubClose = () => {
            dispatch(taskHubDisconnected())
        }

        const handleHubReconnecting = () => {
            dispatch(taskHubConnecting())
        }

        const handleHubReconnected = () => {
            dispatch(taskHubConnected())
        }

        taskHubApi.onClose(handleHubClose)
        taskHubApi.onReconnecting(handleHubReconnecting)
        taskHubApi.onReconnected(handleHubReconnected)
    }, [dispatch])

    useEffect(() => {
        const handleUpdateStatus = (isSynchronizing: boolean) => {
            dispatch(toggleSaveTaskPending(isSynchronizing))
        }

        const handleHubHeartbeat = () => {
            console.log('Heartbeat')
        }

        const handleUpdateTasks = (tasks: TaskModel[]) => {
            const { tasksToNotify, versionsToNotify } =
                taskSyncService.updateTasks(tasks)

            console.log('WS Update tasks: ', {
                WS: tasks,
                tasksToNotify,
                versionsToNotify,
            })

            if (tasksToNotify.length > 0) {
                dispatch(updateTasks(tasksToNotify))
            }

            if (versionsToNotify.length > 0) {
                dispatch(updateVersions(versionsToNotify))
            }
        }

        taskSyncService.subscribeStatusUpdate(handleUpdateStatus)
        taskHubApi.onUpdate(handleUpdateTasks)
        taskHubApi.onHeartbeat(handleHubHeartbeat)

        return () => {
            taskSyncService.unsubscribeStatusUpdate(handleUpdateStatus)
            taskHubApi.offUpdate(handleUpdateTasks)
            taskHubApi.offHeartbeat(handleHubHeartbeat)
        }
    }, [dispatch])
}
