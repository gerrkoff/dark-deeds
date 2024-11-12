import { useCallback } from 'react'
import { useAppDispatch } from '../../hooks'
import { syncTasks, syncVersions } from '../../overview/redux/overview-slice'
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
            const { tasksToNotify, versionsToNotify } =
                taskSyncService.updateTasks(tasks)

            console.log(`[${new Date().toISOString()}] Tasks online update:`, {
                tasks,
                tasksToNotify,
                versionsToNotify,
            })

            if (tasksToNotify.length > 0) {
                dispatch(syncTasks(tasksToNotify))
            }

            if (versionsToNotify.length > 0) {
                dispatch(syncVersions(versionsToNotify))
            }
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
