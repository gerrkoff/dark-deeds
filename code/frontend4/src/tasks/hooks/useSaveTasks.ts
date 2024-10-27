import { useEffect } from 'react'
import { useAppDispatch } from '../../hooks'
import { toggleSaveTaskPending } from '../../status-panel/redux/status-panel-slice'
import { taskSyncService } from '../services/TaskSyncService'

export function useSaveTasks() {
    const dispatch = useAppDispatch()

    useEffect(() => {
        const subscription = (isSynchronizing: boolean) => {
            dispatch(toggleSaveTaskPending(isSynchronizing))
        }

        taskSyncService.subscribe(subscription)

        return () => {
            taskSyncService.unsubscribe(subscription)
        }
    }, [dispatch])
}
