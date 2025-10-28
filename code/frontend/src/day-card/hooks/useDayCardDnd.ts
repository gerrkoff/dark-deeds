import { useCallback, useEffect, useState } from 'react'
import { TaskModel } from '../../tasks/models/TaskModel'
import { DayCardItemDndContext, DropZoneIdType } from '../models/DayCardDndContext'
import { clearDraggedTask, useDayCardDndItemContext } from './useDayCardDndItemContext'

interface Output {
    draggedTaskUid: string | null
    dropzoneHighlightedTaskUid: DropZoneIdType | null
    handleListDragLeave: () => void
    itemDndContext: DayCardItemDndContext
}

interface Props {
    tasks: TaskModel[]
    onSaveTasks: (tasks: TaskModel[]) => void
    onTransformDrop: (task: TaskModel) => TaskModel
}

export function useDayCardDnd({ tasks, onSaveTasks, onTransformDrop }: Props): Output {
    const [draggedTaskUid, setDraggedTaskUid] = useState<string | null>(null)
    const [dropzoneHighlightedTaskUid, setDropzoneHighlightedTaskUid] = useState<DropZoneIdType | null>(null)

    const setupDropzoneHighlightedTaskUid = useCallback((dropzoneHighlightedTaskUid: DropZoneIdType | null) => {
        debouncedSetValue(dropzoneHighlightedTaskUid, setDropzoneHighlightedTaskUid)
    }, [])

    useEffect(() => {
        const handleDrop = () => {
            setDraggedTaskUid(null)
            setupDropzoneHighlightedTaskUid(null)
            clearDraggedTask()
        }

        const handleDragEnd = () => {
            if (window._isDragEnabled) {
                return
            }
            setDraggedTaskUid(null)
            setupDropzoneHighlightedTaskUid(null)
            clearDraggedTask()
        }

        document.addEventListener('drop', handleDrop)
        document.addEventListener('dragend', handleDragEnd)

        return () => {
            document.removeEventListener('drop', handleDrop)
            document.removeEventListener('dragend', handleDragEnd)
            clearDebouncedSetValue()
        }
    }, [setupDropzoneHighlightedTaskUid])

    const handleListDragLeave = useCallback(() => {
        setupDropzoneHighlightedTaskUid(null)
    }, [setupDropzoneHighlightedTaskUid])

    const { context: itemDndContext } = useDayCardDndItemContext({
        tasks,
        onSaveTasks,
        onTransformDrop,
        setDraggedTaskUid,
        setDropzoneHighlightedTaskUid: setupDropzoneHighlightedTaskUid,
    })

    return {
        draggedTaskUid,
        dropzoneHighlightedTaskUid,
        handleListDragLeave,
        itemDndContext,
    }
}

let dropzoneHighlightUpdateTimer: ReturnType<typeof setTimeout> | undefined
let lastDropzoneHighlightValue: DropZoneIdType | null | undefined

function debouncedSetValue(value: DropZoneIdType | null, setter: (value: DropZoneIdType | null) => void): void {
    if (!dropzoneHighlightUpdateTimer) {
        dropzoneHighlightUpdateTimer = setTimeout(() => {
            const lastValue = lastDropzoneHighlightValue
            if (lastValue !== undefined) {
                setter(lastValue)
            }
            dropzoneHighlightUpdateTimer = undefined
            lastDropzoneHighlightValue = undefined
        }, 16)
    }

    lastDropzoneHighlightValue = value
}

function clearDebouncedSetValue(): void {
    if (dropzoneHighlightUpdateTimer) {
        clearTimeout(dropzoneHighlightUpdateTimer)
        dropzoneHighlightUpdateTimer = undefined
        lastDropzoneHighlightValue = undefined
    }
}
