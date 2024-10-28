import { useCallback, useEffect } from 'react'
import { useAppDispatch } from '../../hooks'
import { toggleSaveTaskPending } from '../../status-panel/redux/status-panel-slice'
import { updateTasks } from '../../overview/redux/overview-slice'
import { taskSubscriptionService } from '../services/TaskSubscriptionService'
import { TaskModel } from '../models/TaskModel'
import { taskHubApi } from '../api/TaskHubApi'
import { taskSyncService } from '../services/TaskSyncService'

interface Output {
    connectTasksHub: () => Promise<void>
    disconnectTasksHub: () => Promise<void>
}

export function useTasksHub(): Output {
    const dispatch = useAppDispatch()

    useEffect(() => {
        taskHubApi.init()
    }, [])

    useEffect(() => {
        const handleHubClose = () => {
            console.log('Closed')
        }

        const handleHubReconnecting = () => {
            console.log('Reconnecting')
        }

        const handleHubReconnected = () => {
            console.log('Reconnected')
        }

        taskHubApi.onClose(handleHubClose)
        taskHubApi.onReconnecting(handleHubReconnecting)
        taskHubApi.onReconnected(handleHubReconnected)
    }, [])

    useEffect(() => {
        const handleUpdateStatus = (isSynchronizing: boolean) => {
            dispatch(toggleSaveTaskPending(isSynchronizing))
        }

        const handleUpdateTasks = (tasks: TaskModel[]) => {
            dispatch(updateTasks(tasks))
        }

        const handleHubHeartbeat = () => {
            console.log('Heartbeat')
        }

        taskSubscriptionService.subscribeStatusUpdate(handleUpdateStatus)
        taskSubscriptionService.subscribeTaskUpdate(handleUpdateTasks)

        taskHubApi.onHeartbeat(handleHubHeartbeat)

        taskSyncService.hubSubscribe()

        return () => {
            taskSubscriptionService.unsubscribeStatusUpdate(handleUpdateStatus)
            taskSubscriptionService.unsubscribeTaskUpdate(handleUpdateTasks)
            taskHubApi.offHeartbeat()
            taskSyncService.hubUnsubscribe()
        }
    }, [dispatch])

    const connectTasksHub = useCallback(async () => {
        await taskHubApi.start()
    }, [])

    const disconnectTasksHub = useCallback(async () => {
        await taskHubApi.stop()
    }, [])

    return {
        connectTasksHub,
        disconnectTasksHub,
    }
}
