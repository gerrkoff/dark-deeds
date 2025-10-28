import { useCallback, useMemo, useState } from 'react'
import { TaskEditModalContext, TaskEditModalContent } from '../models/TaskEditModalContext'

interface Output {
    taskEditModalContext: TaskEditModalContext | null
    openTaskEditModal: (content: TaskEditModalContent) => void
}

type InternalContext = { content: TaskEditModalContent; isShown: boolean } | null

export function useEditTaskModal(): Output {
    const [context, setContext] = useState<InternalContext>(null)

    const openTaskEditModal = useCallback((content: TaskEditModalContent) => {
        setContext({
            isShown: true,
            content,
        })
    }, [])

    const taskEditModalContext = useMemo((): TaskEditModalContext | null => {
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
    }
}
