import { useEffect } from 'react'
import { useAppDispatch } from '../../hooks'
import { toggleSaveTaskPending } from '../../status-panel/redux/status-panel-slice'
import { TaskVersionModel } from '../models/TaskVersionModel'
import {
    updateTasks,
    updateVersions,
} from '../../overview/redux/overview-slice'
import { taskSubscriptionService } from '../services/TaskSubscriptionService'
import { TaskModel } from '../models/TaskModel'

export function useSaveTasks() {
    const dispatch = useAppDispatch()

    useEffect(() => {
        const updateStatusCallback = (isSynchronizing: boolean) => {
            dispatch(toggleSaveTaskPending(isSynchronizing))
        }

        const updateVersionCallback = (versions: TaskVersionModel[]) => {
            dispatch(updateVersions(versions))
        }

        const updateTaskCallback = (tasks: TaskModel[]) => {
            dispatch(updateTasks(tasks))
        }

        taskSubscriptionService.subscribeStatusUpdate(updateStatusCallback)
        taskSubscriptionService.subscribeVersionsUpdate(updateVersionCallback)
        taskSubscriptionService.subscribeTaskUpdate(updateTaskCallback)

        return () => {
            taskSubscriptionService.unsubscribeStatusUpdate(
                updateStatusCallback,
            )
            taskSubscriptionService.unsubscribeVersionsUpdate(
                updateVersionCallback,
            )
            taskSubscriptionService.unsubscribeTaskUpdate(updateTaskCallback)
        }
    }, [dispatch])
}
