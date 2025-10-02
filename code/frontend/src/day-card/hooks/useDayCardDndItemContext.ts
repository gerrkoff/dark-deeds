import { useMemo } from 'react'
import { TaskModel } from '../../tasks/models/TaskModel'
import {
    DayCardItemDndContext,
    dropZoneBottomId,
    DropZoneDirectionType,
    DropZoneIdType,
} from '../models/DayCardDndContext'

interface Output {
    context: DayCardItemDndContext
}

interface Props {
    tasks: TaskModel[]
    onSaveTasks: (tasks: TaskModel[]) => void
    onTransformDrop: (task: TaskModel) => TaskModel
    setDraggedTaskUid: (uid: string | null) => void
    setDropzoneHighlightedTaskUid: (uid: DropZoneIdType | null) => void
}

let draggedTaskPayload: TaskModel | null = null

export function useDayCardDndItemContext({
    tasks,
    onSaveTasks,
    onTransformDrop,
    setDraggedTaskUid,
    setDropzoneHighlightedTaskUid,
}: Props): Output {
    const context = useMemo(
        () => ({
            handleItemDragStart(task: TaskModel): void {
                setDraggedTaskUid(task.uid)
                draggedTaskPayload = task
            },

            handleItemDragOver(e: DragEvent, uid: DropZoneIdType, direction: 'above' | 'below'): void {
                if (!draggedTaskPayload) {
                    return
                }

                const itemIndex = findCorrespondingIndex(tasks, uid, direction)

                if (!canDrop(tasks, draggedTaskPayload, itemIndex)) {
                    setDropzoneHighlightedTaskUid(null)
                    return
                }

                e.preventDefault()

                setDropzoneHighlightedTaskUid(itemIndex === tasks.length ? dropZoneBottomId : tasks[itemIndex].uid)
            },

            handleItemDrop(e: DragEvent, dropZoneId: DropZoneIdType, direction: 'above' | 'below'): void {
                if (!draggedTaskPayload) {
                    return
                }

                const itemIndex = findCorrespondingIndex(tasks, dropZoneId, direction)

                if (!canDrop(tasks, draggedTaskPayload, itemIndex)) {
                    return
                }

                e.preventDefault()

                const order =
                    tasks.length === 0
                        ? 0
                        : itemIndex === tasks.length
                          ? tasks[tasks.length - 1].order + 0.5
                          : tasks[itemIndex].order - 0.5

                const transformed = onTransformDrop(draggedTaskPayload)

                const updatedItem = {
                    ...transformed,
                    order,
                }

                onSaveTasks([updatedItem])
            },
        }),
        [onSaveTasks, onTransformDrop, setDraggedTaskUid, setDropzoneHighlightedTaskUid, tasks],
    )

    return { context }
}

export function clearDraggedTask(): void {
    draggedTaskPayload = null
}

function canDrop(tasks: TaskModel[], draggedTask: TaskModel, droppingIndex: number): boolean {
    const draggedItemIndex = tasks.findIndex(x => x.uid === draggedTask.uid)

    if (draggedItemIndex === -1) {
        return true
    }

    if (droppingIndex === draggedItemIndex || droppingIndex - draggedItemIndex === 1) {
        return false
    }

    return true
}

function findCorrespondingIndex(tasks: TaskModel[], uid: DropZoneIdType, direction: DropZoneDirectionType): number {
    if (uid === dropZoneBottomId) {
        if (direction === 'above') {
            return tasks.length
        } else {
            throw new Error(`Invalid direction ${direction}`)
        }
    }

    const index = tasks.findIndex(x => x.uid === uid)

    if (index === -1) {
        throw new Error(`Task with uid ${uid} not found`)
    }

    const newIndex = direction === 'above' ? index : index + 1

    return newIndex
}
