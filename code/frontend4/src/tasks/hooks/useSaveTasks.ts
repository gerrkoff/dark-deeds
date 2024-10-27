import { useEffect } from 'react'
import { useAppDispatch } from '../../hooks'
import { toggleSaveTaskPending } from '../../status-panel/redux/status-panel-slice'
import { taskSyncService } from '../services/TaskSyncService'
import { TaskVersionModel } from '../models/TaskVersionModel'
import { updateVersions } from '../../overview/redux/overview-slice'

export function useSaveTasks() {
    const dispatch = useAppDispatch()

    useEffect(() => {
        const updateStatusCallback = (isSynchronizing: boolean) => {
            dispatch(toggleSaveTaskPending(isSynchronizing))
        }

        const updateVersionCallback = (versions: TaskVersionModel[]) => {
            dispatch(updateVersions(versions))
        }

        taskSyncService.subscribeStatusUpdate(updateStatusCallback)
        taskSyncService.subscribeVersionsUpdate(updateVersionCallback)

        return () => {
            taskSyncService.unsubscribeStatusUpdate(updateStatusCallback)
            taskSyncService.unsubscribeVersionsUpdate(updateVersionCallback)
        }
    }, [dispatch])
}
