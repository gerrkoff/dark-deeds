import { useCallback, useEffect, useMemo, useState } from 'react'
import { TaskModel } from '../../tasks/models/TaskModel'
import {
    DayCardItemDndContext,
    DropZoneDirectionType,
    DropZoneIdType,
    isBottomDropZoneId,
} from '../models/DayCardDndContext'
import { throttle } from '../../common/utils/throttle'

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
        const handleItemDragStart = (e: DragEvent, task: TaskModel) => {
            e.dataTransfer?.setData('dd/item', JSON.stringify(task))
            setDraggedTaskUid(task.uid)
        }

        const handleItemDragOver = (
            uid: DropZoneIdType,
            direction: DropZoneDirectionType,
        ) => {
            const itemIndex = findCorrespondingIndex(tasks, uid, direction)
            setDropzoneHighlightedTaskUid(
                itemIndex === tasks.length
                    ? 'bottom-dropzone'
                    : tasks[itemIndex].uid,
            )
        }

        const handleItemDragOverThrottled = throttle(handleItemDragOver)

        const handleItemDrop = (
            e: DragEvent,
            dropZoneId: DropZoneIdType,
            direction: DropZoneDirectionType,
        ) => {
            if (!e.dataTransfer) {
                return
            }

            const draggedTask: TaskModel = JSON.parse(
                e.dataTransfer.getData('dd/item'),
            )

            const itemIndex = findCorrespondingIndex(
                tasks,
                dropZoneId,
                direction,
            )
            const order =
                tasks.length === 0
                    ? 0
                    : itemIndex === tasks.length
                      ? tasks[tasks.length - 1].order + 0.5
                      : tasks[itemIndex].order - 0.5

            const updatedItem = {
                ...draggedTask,
                date: date?.valueOf() ?? null,
                order,
            }

            onSaveTasks([updatedItem])
        }

        return {
            handleItemDragStart,
            handleItemDragOver: handleItemDragOverThrottled,
            handleItemDrop,
        }
    }, [tasks, date, onSaveTasks])

    return {
        draggedTaskUid,
        dropzoneHighlightedTaskUid,
        handleListDragLeave,
        itemDndContext,
    }
}

function findCorrespondingIndex(
    tasks: TaskModel[],
    uid: DropZoneIdType,
    direction: DropZoneDirectionType,
): number {
    if (isBottomDropZoneId(uid)) {
        if (direction === 'above') {
            return tasks.length
        } else {
            throw new Error(`Invalid direction ${direction}`)
        }
    }

    const index = tasks.findIndex(task => task.uid === uid)

    if (index === -1) {
        throw new Error(`Task with uid ${uid} not found`)
    }

    const newIndex = direction === 'above' ? index : index + 1

    return newIndex
}
