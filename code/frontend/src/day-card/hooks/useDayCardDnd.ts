import { useCallback, useEffect, useState } from 'react'
import { TaskModel } from '../../tasks/models/TaskModel'
import {
    DayCardItemDndContext,
    DropZoneIdType,
} from '../models/DayCardDndContext'
import {
    clearDraggedTask,
    useDayCardDndItemContext,
} from './useDayCardDndItemContext'

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

export function useDayCardDnd({
    tasks,
    onSaveTasks,
    onTransformDrop,
}: Props): Output {
    const [draggedTaskUid, setDraggedTaskUid] = useState<string | null>(null)
    const [dropzoneHighlightedTaskUid, setDropzoneHighlightedTaskUid] =
        useState<DropZoneIdType | null>(null)

    useEffect(() => {
        const handleDrop = () => {
            setDraggedTaskUid(null)
            setDropzoneHighlightedTaskUid(null)
            clearDraggedTask()
        }

        const handleDragEnd = () => {
            if (window._isDragEnabled) {
                return
            }
            setDraggedTaskUid(null)
            setDropzoneHighlightedTaskUid(null)
            clearDraggedTask()
        }

        document.addEventListener('drop', handleDrop)
        document.addEventListener('dragend', handleDragEnd)

        return () => {
            document.addEventListener('drop', handleDrop)
            document.removeEventListener('dragend', handleDragEnd)
        }
    }, [])

    const handleListDragLeave = useCallback(() => {
        setDropzoneHighlightedTaskUid(null)
    }, [])

    const { context: itemDndContext } = useDayCardDndItemContext({
        tasks,
        onSaveTasks,
        onTransformDrop,
        setDraggedTaskUid,
        setDropzoneHighlightedTaskUid,
    })

    return {
        draggedTaskUid,
        dropzoneHighlightedTaskUid,
        handleListDragLeave,
        itemDndContext,
    }
}
