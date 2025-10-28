import { useEffect, useRef } from 'react'
import { DayCardItemDndContext } from '../models/DayCardDndContext'

interface Output {
    listRef: React.RefObject<HTMLUListElement>
    lastItemRef: React.RefObject<HTMLLIElement>
}

interface Props {
    handleListDragLeave: () => void
    itemDndContext: DayCardItemDndContext
}

export function useDayCardDndList({ handleListDragLeave, itemDndContext }: Props): Output {
    const listRef = useRef<HTMLUListElement>(null)
    const lastItemRef = useRef<HTMLLIElement>(null)

    useEffect(() => {
        const element = listRef.current

        if (!element) {
            return
        }

        element.addEventListener('dragleave', handleListDragLeave)

        return () => {
            element.removeEventListener('dragleave', handleListDragLeave)
        }
    }, [handleListDragLeave])

    useEffect(() => {
        const element = lastItemRef.current

        if (!element) {
            return
        }

        const { handleItemDragOver, handleItemDrop, bottomDropZoneId } = itemDndContext

        const handleDragOver = (e: DragEvent) => {
            handleItemDragOver(e, bottomDropZoneId, 'above')
        }

        const handleDrop = (e: DragEvent) => {
            handleItemDrop(e, bottomDropZoneId, 'above')
        }

        element.addEventListener('dragover', handleDragOver)
        element.addEventListener('drop', handleDrop)

        return () => {
            element.removeEventListener('dragover', handleDragOver)
            element.removeEventListener('drop', handleDrop)
        }
    }, [itemDndContext])

    return {
        listRef,
        lastItemRef,
    }
}
