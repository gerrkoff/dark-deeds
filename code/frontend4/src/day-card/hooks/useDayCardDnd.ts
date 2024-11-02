import { useCallback, useEffect, useMemo, useState } from 'react'
import { TaskModel } from '../../tasks/models/TaskModel'
import {
    DayCardItemDndContext,
    DropZoneIdType,
} from '../models/DayCardDndContext'
import { TaskDndService } from '../services/TaskDndService'

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
            TaskDndService.clear()
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

    const itemDndContext: DayCardItemDndContext = useMemo(() => {
        const taskDndService = new TaskDndService(
            tasks,
            date,
            onSaveTasks,
            setDraggedTaskUid,
            setDropzoneHighlightedTaskUid,
        )

        return {
            handleItemDragStart:
                taskDndService.handleItemDragStart.bind(taskDndService),
            handleItemDragOver:
                taskDndService.handleItemDragOver.bind(taskDndService),
            handleItemDrop: taskDndService.handleItemDrop.bind(taskDndService),
        }
    }, [tasks, date, onSaveTasks])

    return {
        draggedTaskUid,
        dropzoneHighlightedTaskUid,
        handleListDragLeave,
        itemDndContext,
    }
}
