import { useCallback, useState } from 'react'
import { TaskModel } from '../../tasks/models/TaskModel'
import { DayCardItemMenuContext } from '../models/DayCardItemMenuContext'

interface Output {
    itemMenuContext: DayCardItemMenuContext | null
    openItemMenu: (e: React.MouseEvent<HTMLElement>, task: TaskModel) => void
    closeItemMenu: () => void
}

interface Props {
    containerRef?: React.RefObject<HTMLDivElement>
}

export function useDayCardItemMenu({ containerRef }: Props): Output {
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

    return {
        itemMenuContext,
        openItemMenu,
        closeItemMenu,
    }
}
