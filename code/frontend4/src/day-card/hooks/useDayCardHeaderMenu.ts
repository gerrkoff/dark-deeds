import { useCallback, useState } from 'react'
import { DayCardHeaderMenuContext } from '../models/DayCardHeaderMenuContext'

interface Output {
    headerMenuContext: DayCardHeaderMenuContext | null
    openHeaderMenu: (e: React.MouseEvent<HTMLElement>, date: Date) => void
    closeHeaderMenu: () => void
    onAddTaskForDate: (date: Date) => void
}

interface Props {
    containerRef: React.RefObject<HTMLDivElement>
    openTaskEditModalForDate: (date: Date) => void
}

export function useDayCardHeaderMenu({
    containerRef,
    openTaskEditModalForDate,
}: Props): Output {
    const [context, setContext] = useState<DayCardHeaderMenuContext | null>(
        null,
    )

    const openMenu = useCallback(
        (e: React.MouseEvent<HTMLElement>, date: Date) => {
            e.preventDefault()

            if (!containerRef.current) {
                return
            }

            const targetRect = containerRef.current.getBoundingClientRect()
            const x = e.clientX - targetRect.left
            const y = e.clientY - targetRect.top
            setContext({
                date,
                position: { x, y },
            })
        },
        [containerRef],
    )

    const closeMenu = useCallback(() => setContext(null), [])

    const addTaskForDate = useCallback(
        (date: Date) => {
            closeMenu()
            openTaskEditModalForDate(date)
        },
        [closeMenu, openTaskEditModalForDate],
    )

    return {
        headerMenuContext: context,
        openHeaderMenu: openMenu,
        closeHeaderMenu: closeMenu,
        onAddTaskForDate: addTaskForDate,
    }
}
