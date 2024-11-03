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
    date: Date | null
    tasks: TaskModel[]
    onSaveTasks: (tasks: TaskModel[]) => void
}

export function useDayCardDnd({ date, tasks, onSaveTasks }: Props): Output {
    const [draggedTaskUid, setDraggedTaskUid] = useState<string | null>(null)
    const [dropzoneHighlightedTaskUid, setDropzoneHighlightedTaskUid] =
        useState<DropZoneIdType | null>(null)

    useEffect(() => {
        const handleClear = () => {
            setDraggedTaskUid(null)
            setDropzoneHighlightedTaskUid(null)
            clearDraggedTask()
        }

        document.addEventListener('drop', handleClear)
        document.addEventListener('dragend', handleClear)

        return () => {
            document.addEventListener('drop', handleClear)
            document.removeEventListener('dragend', handleClear)
        }
    }, [])

    const handleListDragLeave = useCallback(() => {
        setDropzoneHighlightedTaskUid(null)
    }, [])

    const { context: itemDndContext } = useDayCardDndItemContext({
        date,
        tasks,
        onSaveTasks,
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
