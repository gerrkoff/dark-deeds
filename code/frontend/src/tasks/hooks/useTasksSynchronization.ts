import { useCallback } from 'react'
import { useAppDispatch } from '../../hooks'
import { syncTasks, updateTaskVersions } from '../../overview/redux/overview-slice'
import { TaskModel } from '../models/TaskModel'
import { taskSyncService } from '../services/TaskSyncService'
import { reloadOverviewTasks } from '../../overview/redux/overview-thunk'
import { unwrapResult } from '@reduxjs/toolkit'
import { addToast } from '../../toasts/redux/toasts-slice'
import { TaskVersionModel } from '../models/TaskVersionModel'

interface Output {
    processTasksOnlineUpdate: (tasks: TaskModel[]) => void
    processTaskSaveFinish: (notSaved: number, savedTasks: TaskVersionModel[]) => void
    reloadTasks: () => void
}

export function useTasksSynchronization(): Output {
    const dispatch = useAppDispatch()

    const processTasksOnlineUpdate = useCallback(
        (tasks: TaskModel[]) => {
            const tasksConflicted = taskSyncService.processTasksOnlineUpdate(tasks)

            console.log(`[${new Date().toISOString()}] Tasks online update:`, {
                tasks,
                tasksConflicted,
            })

            dispatch(
                syncTasks({
                    tasks,
                }),
            )

            if (tasksConflicted.length > 0) {
                for (const task of tasksConflicted) {
                    dispatch(
                        addToast({
                            text: `Task "${task.title}" was updated by another client`,
                        }),
                    )
                }
            }
        },
        [dispatch],
    )

    const reloadTasks = useCallback(async () => {
        const reloadOverviewTasksResult = await dispatch(reloadOverviewTasks())
        const tasks = unwrapResult(reloadOverviewTasksResult)
        processTasksOnlineUpdate(tasks)
    }, [dispatch, processTasksOnlineUpdate])

    const processTaskSaveFinish = useCallback(
        (notSaved: number, savedTasks: TaskVersionModel[]) => {
            if (notSaved > 0) {
                dispatch(
                    addToast({
                        text: `Failed to save ${notSaved} tasks`,
                        category: 'task-save-failed',
                    }),
                )
            }

            if (savedTasks.length > 0) {
                dispatch(updateTaskVersions(savedTasks))
            }
        },
        [dispatch],
    )

    return {
        processTasksOnlineUpdate,
        processTaskSaveFinish,
        reloadTasks,
    }
}
