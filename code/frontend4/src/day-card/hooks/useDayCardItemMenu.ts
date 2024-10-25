import { useCallback, useState } from 'react'
import { TaskModel } from '../../tasks/models/TaskModel'
import { DayCardItemMenuContext } from '../models/DayCardItemMenuContext'
import { useChangeHandlers } from '../../tasks/hooks/useChangeHandlers'

interface Output {
    itemMenuContext: DayCardItemMenuContext | null
    openItemMenu: (e: React.MouseEvent<HTMLElement>, task: TaskModel) => void
    closeItemMenu: () => void
    toggleTaskCompleted: (task: TaskModel) => void
    deleteTask: (task: TaskModel) => void
    editTask: (task: TaskModel) => void
}

interface Props {
    containerRef: React.RefObject<HTMLDivElement>
    saveTasks: (tasks: TaskModel[]) => void
    openTaskEditModal: (task: TaskModel) => void
}

export function useDayCardItemMenu({
    containerRef,
    saveTasks,
    openTaskEditModal,
}: Props): Output {
    const [context, setContext] = useState<DayCardItemMenuContext | null>(null)

    const openMenu = useCallback(
        (e: React.MouseEvent<HTMLElement>, task: TaskModel) => {
            e.preventDefault()

            if (!containerRef.current) {
                return
            }

            const targetRect = containerRef.current.getBoundingClientRect()
            const x = e.clientX - targetRect.left
            const y = e.clientY - targetRect.top
            setContext({
                task,
                position: { x, y },
            })
        },
        [containerRef],
    )

    const closeMenu = useCallback(() => setContext(null), [])

    const saveTaskAndCloseMenu = useCallback(
        (tasks: TaskModel[]) => {
            saveTasks(tasks)
            closeMenu()
        },
        [closeMenu, saveTasks],
    )

    const { toggleTaskCompleted, deleteTask } = useChangeHandlers({
        saveTasks: saveTaskAndCloseMenu,
    })

    const editTask = useCallback(
        (task: TaskModel) => {
            closeMenu()
            openTaskEditModal(task)
        },
        [closeMenu, openTaskEditModal],
    )

    return {
        itemMenuContext: context,
        openItemMenu: openMenu,
        closeItemMenu: closeMenu,
        toggleTaskCompleted,
        deleteTask,
        editTask,
    }
}
