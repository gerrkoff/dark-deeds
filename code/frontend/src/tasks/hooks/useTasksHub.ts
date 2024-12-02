import { useEffect } from 'react'
import { useAppDispatch } from '../../hooks'
import {
    taskHubConnected,
    taskHubConnecting,
    taskHubDisconnected,
    toggleSaveTaskPending,
} from '../../status-panel/redux/status-panel-slice'
import { taskHubApi } from '../api/TaskHubApi'
import { taskSyncService } from '../services/TaskSyncService'
import { useTasksSynchronization } from './useTasksSynchronization'
import { addToast } from '../../toasts/redux/toasts-slice'
import { uuidv4 } from '../../common/utils/uuidv4'

export function useTasksHub() {
    const dispatch = useAppDispatch()

    useEffect(() => {
        taskHubApi.init()
    }, [])

    const { processTasksOnlineUpdate, reloadTasks } = useTasksSynchronization()

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

        const handleTaskSaveFinish = (notSaved: number) => {
            if (notSaved > 0) {
                dispatch(
                    addToast({
                        id: uuidv4(),
                        text: `Failed to save ${notSaved} tasks`,
                    }),
                )
            }
        }

        taskHubApi.onClose(handleHubClose)
        taskHubApi.onReconnecting(handleHubReconnecting)
        taskHubApi.onReconnected(handleHubReconnected)
        taskHubApi.onUpdate(processTasksOnlineUpdate)
        taskHubApi.onHeartbeat(handleHeartbeat)
        taskSyncService.subscribeStatusUpdate(handleUpdateStatus)
        taskSyncService.subscribeSaveFinish(handleTaskSaveFinish)

        return () => {
            taskHubApi.offUpdate(processTasksOnlineUpdate)
            taskHubApi.offHeartbeat(handleHeartbeat)
            taskSyncService.unsubscribeStatusUpdate(handleUpdateStatus)
            taskSyncService.unsubscribeSaveFinish(handleTaskSaveFinish)
        }
    }, [dispatch, processTasksOnlineUpdate, reloadTasks])
}
