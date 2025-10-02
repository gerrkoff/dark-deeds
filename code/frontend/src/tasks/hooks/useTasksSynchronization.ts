import { useCallback } from 'react'
import { useAppDispatch } from '../../hooks'
import { syncTasks } from '../../overview/redux/overview-slice'
import { TaskModel } from '../models/TaskModel'
import { taskSyncService } from '../services/TaskSyncService'
import { reloadOverviewTasks } from '../../overview/redux/overview-thunk'
import { unwrapResult } from '@reduxjs/toolkit'

interface Output {
    processTasksOnlineUpdate: (tasks: TaskModel[]) => void
    reloadTasks: () => void
}

export function useTasksSynchronization(): Output {
    const dispatch = useAppDispatch()

    const processTasksOnlineUpdate = useCallback(
        (tasks: TaskModel[]) => {
            const { tasksToNotify, versionsToNotify } = taskSyncService.updateTasks(tasks)

            console.log(`[${new Date().toISOString()}] Tasks online update:`, {
                tasks,
                tasksToNotify,
                versionsToNotify,
            })

            dispatch(
                syncTasks({
                    tasks: tasksToNotify,
                    versions: versionsToNotify,
                }),
            )
        },
        [dispatch],
    )

    const reloadTasks = useCallback(async () => {
        const reloadOverviewTasksResult = await dispatch(reloadOverviewTasks())
        const tasks = unwrapResult(reloadOverviewTasksResult)
        processTasksOnlineUpdate(tasks)
    }, [dispatch, processTasksOnlineUpdate])

    return {
        processTasksOnlineUpdate,
        reloadTasks,
    }
}
