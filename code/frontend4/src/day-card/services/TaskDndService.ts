import { TaskModel } from '../../tasks/models/TaskModel'
import {
    dropZoneBottomId,
    DropZoneDirectionType,
    DropZoneIdType,
} from '../models/DayCardDndContext'

let draggedTaskPayload: TaskModel | null = null

export class TaskDndService {
    constructor(
        private tasks: TaskModel[],
        private date: Date | null,
        private onSaveTasks: (tasks: TaskModel[]) => void,
        private setDraggedTaskUid: (uid: string | null) => void,
        private setDropzoneHighlightedTaskUid: (
            uid: DropZoneIdType | null,
        ) => void,
    ) {}

    static clear(): void {
        draggedTaskPayload = null
    }

    handleItemDragStart(task: TaskModel): void {
        this.setDraggedTaskUid(task.uid)
        draggedTaskPayload = task
    }

    handleItemDragOver(
        e: DragEvent,
        uid: DropZoneIdType,
        direction: 'above' | 'below',
    ): void {
        if (!draggedTaskPayload) {
            return
        }

        const itemIndex = this.findCorrespondingIndex(uid, direction)

        if (!this.canDrop(draggedTaskPayload, itemIndex)) {
            this.setDropzoneHighlightedTaskUid(null)
            return
        }

        e.preventDefault()

        this.setDropzoneHighlightedTaskUid(
            itemIndex === this.tasks.length
                ? dropZoneBottomId
                : this.tasks[itemIndex].uid,
        )
    }

    handleItemDrop(
        e: DragEvent,
        dropZoneId: DropZoneIdType,
        direction: 'above' | 'below',
    ): void {
        if (!draggedTaskPayload) {
            return
        }

        const itemIndex = this.findCorrespondingIndex(dropZoneId, direction)

        if (!this.canDrop(draggedTaskPayload, itemIndex)) {
            return
        }

        e.preventDefault()

        const order =
            this.tasks.length === 0
                ? 0
                : itemIndex === this.tasks.length
                  ? this.tasks[this.tasks.length - 1].order + 0.5
                  : this.tasks[itemIndex].order - 0.5

        const updatedItem = {
            ...draggedTaskPayload,
            order,
            date: this.date ? this.date.valueOf() : null,
        }

        this.onSaveTasks([updatedItem])
    }

    private canDrop(draggedTask: TaskModel, droppingIndex: number): boolean {
        const draggedItemIndex = this.tasks.findIndex(
            x => x.uid === draggedTask.uid,
        )

        if (draggedItemIndex === -1) {
            return true
        }

        if (
            droppingIndex === draggedItemIndex ||
            droppingIndex - draggedItemIndex === 1
        ) {
            return false
        }

        return true
    }

    private findCorrespondingIndex(
        uid: DropZoneIdType,
        direction: DropZoneDirectionType,
    ): number {
        if (uid === dropZoneBottomId) {
            if (direction === 'above') {
                return this.tasks.length
            } else {
                throw new Error(`Invalid direction ${direction}`)
            }
        }

        const index = this.tasks.findIndex(task => task.uid === uid)

        if (index === -1) {
            throw new Error(`Task with uid ${uid} not found`)
        }

        const newIndex = direction === 'above' ? index : index + 1

        return newIndex
    }
}
