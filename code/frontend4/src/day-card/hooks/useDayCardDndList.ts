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

export function useDayCardDndList({
    handleListDragLeave,
    itemDndContext,
}: Props): Output {
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

        const { handleItemDragOver, handleItemDrop } = itemDndContext

        const handleDragOver = (e: DragEvent) => {
            e.preventDefault()
            handleItemDragOver('bottom-dropzone', 'above')
        }

        const handleDrop = (e: DragEvent) => {
            e.preventDefault()
            handleItemDrop(e, 'bottom-dropzone', 'above')
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
