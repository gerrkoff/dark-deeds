import { useCallback, useState } from 'react'
import { TaskModel } from '../../tasks/models/TaskModel'
import { TaskEditModalContext } from '../models/TaskEditModalContext'

interface Output {
    taskEditModalContext: TaskEditModalContext
    openTaskEditModal: (task: TaskModel | null) => void
    openTaskEditModalForDate: (date: Date) => void
    closeTaskEditModal: () => void
}

export function useEditTaskModal(): Output {
    const [taskEditModalContext, setTaskEditModalContext] =
        useState<TaskEditModalContext>({
            isShown: false,
            task: null,
            date: null,
        })

    const openTaskEditModal = useCallback((task: TaskModel | null) => {
        setTaskEditModalContext({
            isShown: true,
            task,
            date: null,
        })
    }, [])

    const openTaskEditModalForDate = useCallback((date: Date) => {
        setTaskEditModalContext({
            isShown: true,
            task: null,
            date,
        })
    }, [])

    const closeTaskEditModal = useCallback(
        () =>
            setTaskEditModalContext(old => ({
                ...old,
                isShown: false,
            })),
        [],
    )

    return {
        taskEditModalContext,
        openTaskEditModal,
        openTaskEditModalForDate,
        closeTaskEditModal,
    }
}
