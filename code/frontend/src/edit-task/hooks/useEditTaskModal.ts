import { useCallback, useMemo, useState } from 'react'
import { TaskModel } from '../../tasks/models/TaskModel'
import { TaskEditModalContext } from '../models/TaskEditModalContext'

interface Output {
    taskEditModalContext: TaskEditModalContext | null
    openTaskEditModal: (task: TaskModel | null) => void
    openTaskEditModalForDate: (date: Date) => void
}

export function useEditTaskModal(): Output {
    const [context, setContext] = useState<{
        isShown: boolean
        task: TaskModel | null
        date: Date | null
    } | null>(null)

    const openTaskEditModal = useCallback((task: TaskModel | null) => {
        setContext({
            isShown: true,
            task,
            date: null,
        })
    }, [])

    const openTaskEditModalForDate = useCallback((date: Date) => {
        setContext({
            isShown: true,
            task: null,
            date,
        })
    }, [])

    const taskEditModalContext = useMemo(() => {
        if (context === null) {
            return null
        }

        const close = () =>
            setContext({
                ...context,
                isShown: false,
            })

        const cleanup = () => setContext(null)

        return {
            ...context,
            close,
            cleanup,
        }
    }, [context])

    return {
        taskEditModalContext,
        openTaskEditModal,
        openTaskEditModalForDate,
    }
}
