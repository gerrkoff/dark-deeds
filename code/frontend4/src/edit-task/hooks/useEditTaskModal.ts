import { useCallback, useState } from 'react'
import { TaskModel } from '../../tasks/models/TaskModel'
import { TaskEditModalContext } from '../models/TaskEditModalContext'

interface Output {
    taskEditModalContext: TaskEditModalContext
    openTaskEditModal: (task: TaskModel | null) => void
    closeTaskEditModal: () => void
}

export function useEditTaskModal(): Output {
    const [taskEditModalContext, setTaskEditModalContext] =
        useState<TaskEditModalContext>({
            isShown: false,
            task: null,
        })

    const openTaskEditModal = useCallback((task: TaskModel | null) => {
        setTaskEditModalContext({
            isShown: true,
            task,
        })
    }, [])

    const closeTaskEditModal = useCallback(
        () =>
            setTaskEditModalContext({
                isShown: false,
                task: null,
            }),
        [],
    )

    return {
        taskEditModalContext,
        openTaskEditModal,
        closeTaskEditModal,
    }
}
