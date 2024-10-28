import { useEffect } from 'react'
import { useAppDispatch } from '../../hooks'
import { toggleSaveTaskPending } from '../../status-panel/redux/status-panel-slice'
import { updateTasks } from '../../overview/redux/overview-slice'
import { taskSubscriptionService } from '../services/TaskSubscriptionService'
import { TaskModel } from '../models/TaskModel'

export function useSaveTasks() {
    const dispatch = useAppDispatch()

    useEffect(() => {
        const handleUpdateStatus = (isSynchronizing: boolean) => {
            dispatch(toggleSaveTaskPending(isSynchronizing))
        }

        const handleUpdateTasks = (tasks: TaskModel[]) => {
            dispatch(updateTasks(tasks))
        }

        taskSubscriptionService.subscribeStatusUpdate(handleUpdateStatus)
        taskSubscriptionService.subscribeTaskUpdate(handleUpdateTasks)

        return () => {
            taskSubscriptionService.unsubscribeStatusUpdate(handleUpdateStatus)
            taskSubscriptionService.unsubscribeTaskUpdate(handleUpdateTasks)
        }
    }, [dispatch])
}
