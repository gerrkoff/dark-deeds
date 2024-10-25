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
    containerRef?: React.RefObject<HTMLDivElement>
    saveTasks: (tasks: TaskModel[]) => void
    openTaskEditModal: (task: TaskModel) => void
}

export function useDayCardItemMenu({
    containerRef,
    saveTasks,
    openTaskEditModal,
}: Props): Output {
    const [itemMenuContext, setItemMenuContext] =
        useState<DayCardItemMenuContext | null>(null)

    const openItemMenu = useCallback(
        (e: React.MouseEvent<HTMLElement>, task: TaskModel) => {
            e.preventDefault()

            if (!containerRef?.current) {
                return
            }

            const targetRect = containerRef.current.getBoundingClientRect()
            const x = e.clientX - targetRect.left
            const y = e.clientY - targetRect.top
            setItemMenuContext({
                task,
                position: { x, y },
            })
        },
        [containerRef],
    )

    const closeItemMenu = useCallback(() => setItemMenuContext(null), [])

    const saveTaskAndCloseMenu = useCallback(
        (tasks: TaskModel[]) => {
            saveTasks(tasks)
            closeItemMenu()
        },
        [closeItemMenu, saveTasks],
    )

    const { toggleTaskCompleted, deleteTask } = useChangeHandlers({
        saveTasks: saveTaskAndCloseMenu,
    })

    const editTask = useCallback(
        (task: TaskModel) => {
            closeItemMenu()
            openTaskEditModal(task)
        },
        [closeItemMenu, openTaskEditModal],
    )

    return {
        itemMenuContext,
        openItemMenu,
        closeItemMenu,
        toggleTaskCompleted,
        deleteTask,
        editTask,
    }
}
