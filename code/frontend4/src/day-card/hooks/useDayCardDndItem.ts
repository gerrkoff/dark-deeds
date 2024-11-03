import { useEffect, useRef } from 'react'
import {
    DayCardItemDndContext,
    DropZoneDirectionType,
} from '../models/DayCardDndContext'
import { TaskModel } from '../../tasks/models/TaskModel'

interface Output {
    dragRef: React.RefObject<HTMLElement>
    dropRef: React.RefObject<HTMLLIElement>
}

interface Props {
    task: TaskModel
    itemDndContext: DayCardItemDndContext
}

export function useDayCardDndItem({ task, itemDndContext }: Props): Output {
    const dragRef = useRef<HTMLElement>(null)
    const dropRef = useRef<HTMLLIElement>(null)

    useEffect(() => {
        const element = dragRef.current

        if (!element) {
            return
        }

        const { handleItemDragStart } = itemDndContext

        const handleDragStart = () => {
            // hack for touch devices
            element.classList.add('bg-primary')

            handleItemDragStart(task)
        }

        element.addEventListener('dragstart', handleDragStart)

        return () => {
            element.removeEventListener('dragstart', handleDragStart)
        }
    }, [itemDndContext, task])

    useEffect(() => {
        const element = dropRef.current

        if (!element) {
            return
        }

        const { handleItemDragOver, handleItemDrop } = itemDndContext

        const handleDragOver = (e: DragEvent) => {
            const direction = getDirection(e, element)
            handleItemDragOver(e, task.uid, direction)
        }

        const handleDrop = (e: DragEvent) => {
            const direction = getDirection(e, element)
            handleItemDrop(e, task.uid, direction)
        }

        element.addEventListener('dragover', handleDragOver)
        element.addEventListener('drop', handleDrop)

        return () => {
            element.removeEventListener('dragover', handleDragOver)
            element.removeEventListener('drop', handleDrop)
        }
    }, [itemDndContext, task])

    return {
        dragRef,
        dropRef,
    }
}

function getDirection(
    e: DragEvent,
    element: HTMLElement,
): DropZoneDirectionType {
    const rect = element.getBoundingClientRect()
    return e.clientY - rect.top < rect.height / 2 ? 'above' : 'below'
}
