import { useCallback, useState } from 'react'
import { DayCardHeaderMenuContext } from '../models/DayCardHeaderMenuContext'
import { TaskEditModalContent } from '../../edit-task/models/TaskEditModalContext'

interface Output {
    headerMenuContext: DayCardHeaderMenuContext | null
    openHeaderMenu: (e: React.MouseEvent<HTMLElement>, date: Date) => void
    closeHeaderMenu: () => void
    onAddTaskForDate: (date: Date) => void
}

interface Props {
    containerRef: React.RefObject<HTMLDivElement>
    openTaskEditModal: (content: TaskEditModalContent) => void
}

export function useDayCardMenuHeader({ containerRef, openTaskEditModal }: Props): Output {
    const [context, setContext] = useState<DayCardHeaderMenuContext | null>(null)

    const openMenu = useCallback(
        (e: React.MouseEvent<HTMLElement>, date: Date) => {
            setContext(old => {
                if (!containerRef.current || !(e.target instanceof HTMLElement)) {
                    return old
                }

                if (old !== null) {
                    return null
                }

                const containerRect = containerRef.current.getBoundingClientRect()
                const targetRect = e.target.getBoundingClientRect()
                const x = targetRect.left - containerRect.left
                const y = targetRect.top - containerRect.top + targetRect.height + 4

                return {
                    date,
                    position: { x, y },
                    anchorElement: e.target,
                }
            })
        },
        [containerRef, setContext],
    )

    const closeMenu = useCallback(() => setContext(null), [])

    const addTaskForDate = useCallback(
        (date: Date) => {
            closeMenu()
            openTaskEditModal({ type: 'NEW_FROM_DATE', date })
        },
        [closeMenu, openTaskEditModal],
    )

    return {
        headerMenuContext: context,
        openHeaderMenu: openMenu,
        closeHeaderMenu: closeMenu,
        onAddTaskForDate: addTaskForDate,
    }
}
