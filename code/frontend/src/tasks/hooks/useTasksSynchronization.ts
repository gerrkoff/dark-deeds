import { useCallback } from 'react'
import { useAppDispatch } from '../../hooks'
import { reconcileTasks, syncTasks, updateTaskVersions } from '../../overview/redux/overview-slice'
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

    const reportConflicts = useCallback(
        (tasksConflicted: TaskModel[]) => {
            for (const task of tasksConflicted) {
                dispatch(
                    addToast({
                        text: `Task "${task.title}" was updated by another client`,
                    }),
                )
            }
        },
        [dispatch],
    )

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

            reportConflicts(tasksConflicted)
        },
        [dispatch, reportConflicts],
    )

    const reloadTasks = useCallback(async () => {
        let snapshot: TaskModel[]
        try {
            const reloadOverviewTasksResult = await dispatch(reloadOverviewTasks())
            snapshot = unwrapResult(reloadOverviewTasksResult)
        } catch (error) {
            // Offline or backend unreachable - keep the cached/in-memory tasks and let the
            // reconnect loop retry the reload later.
            console.error('Failed to reload tasks:', error)
            return
        }

        const { tasksConflicted, tasksToApply } = taskSyncService.processTasksOnlineUpdate(snapshot)
        const keepUids = [...new Set([...snapshot.map(task => task.uid), ...taskSyncService.getPendingUids()])]

        console.log(`[${new Date().toISOString()}] Tasks reload:`, {
            snapshot,
            tasksConflicted,
            tasksToApply,
            keepUids,
        })

        dispatch(
            reconcileTasks({
                tasks: tasksToApply,
                keepUids,
            }),
        )

        reportConflicts(tasksConflicted)
    }, [dispatch, reportConflicts])

    const processTaskSaveFinish = useCallback(
        (notSaved: number, savedTasks: TaskVersionModel[], conflictedTasks: TaskModel[]) => {
            if (notSaved > 0) {
                dispatch(
                    addToast({
                        text: `Failed to save ${notSaved} tasks`,
                        category: 'task-save-failed',
                        autoDismissMs: 6000,
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
