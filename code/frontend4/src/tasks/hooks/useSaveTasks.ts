import { useCallback } from 'react'
import { useAppDispatch } from '../../hooks'
import { TaskModel } from '../models/TaskModel'
import { toggleSaveTaskPending } from '../../status-panel/redux/status-panel-slice'
import { taskApi } from '../api/TaskApi'
import { taskSaveService } from '../services/TaskSaveService'

interface Output {
    scheduleTaskSaving: (tasks: TaskModel[]) => void
}

let savingTasksPromise = new Promise<void>(r => r())
let tasksToSave: TaskModel[] = []
let isScheduled = false
let isSaving = false

export function useSaveTasks(): Output {
    const dispatch = useAppDispatch()

    const schedule = useCallback(async (): Promise<void> => {
        if (!isSaving) {
            dispatch(toggleSaveTaskPending(true))
        }

        isScheduled = true
        await savingTasksPromise
        isScheduled = false
        const tasks = tasksToSave
        tasksToSave = []
        isSaving = true
        try {
            await taskApi.saveTasks(tasks)
        } catch {
            tasksToSave = taskSaveService.prependAndFlatten(tasksToSave, tasks)
            savingTasksPromise = schedule()
            return
        }
        isSaving = false

        if (!isScheduled) {
            dispatch(toggleSaveTaskPending(false))
        }
    }, [dispatch])

    const scheduleTaskSaving = useCallback(
        (tasks: TaskModel[]) => {
            tasksToSave = taskSaveService.appendAndFlatten(tasksToSave, tasks)

            if (!isScheduled) {
                savingTasksPromise = schedule()
            }
        },
        [schedule],
    )

    return { scheduleTaskSaving }
}
