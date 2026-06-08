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
    processTaskSaveFinish: (notSaved: number, savedTasks: TaskVersionModel[], conflictedTasks: TaskModel[]) => void
    reloadTasks: () => void
}

export function useTasksSynchronization(): Output {
    const dispatch = useAppDispatch()

    const processTasksOnlineUpdate = useCallback(
        (tasks: TaskModel[]) => {
            const { tasksConflicted, tasksToApply } = taskSyncService.processTasksOnlineUpdate(tasks)

            console.log(`[${new Date().toISOString()}] Tasks online update:`, {
                tasks,
                tasksConflicted,
                tasksToApply,
            })

            dispatch(
                syncTasks({
                    tasks: tasksToApply,
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
        (notSaved: number, savedTasks: TaskVersionModel[], conflictedTasks: TaskModel[]) => {
            if (notSaved > 0) {
                dispatch(
                    addToast({
                        text: `Failed to save ${notSaved} tasks`,
                        category: 'task-save-failed',
                    }),
                )
            }

            if (conflictedTasks.length > 0) {
                console.warn(
                    `[${new Date().toISOString()}] Lost task updates (version conflict, not saved):`,
                    conflictedTasks,
                )
                dispatch(
                    addToast({
                        text: `Lost ${conflictedTasks.length} task update(s)`,
                        category: 'task-save-conflict',
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
