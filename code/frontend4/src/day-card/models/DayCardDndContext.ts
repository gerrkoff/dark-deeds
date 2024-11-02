import { TaskModel } from '../../tasks/models/TaskModel'

export type DropZoneIdType = string | 'bottom-dropzone'
export type DropZoneDirectionType = 'above' | 'below'

export function isBottomDropZoneId(
    id: DropZoneIdType | null,
): id is 'bottom-dropzone' {
    return id === 'bottom-dropzone'
}

export interface DayCardItemDndContext {
    handleItemDragStart: (e: DragEvent, item: TaskModel) => void
    handleItemDragOver: (
        dropZoneId: DropZoneIdType,
        direction: DropZoneDirectionType,
    ) => void
    handleItemDrop: (
        e: DragEvent,
        dropZoneId: DropZoneIdType,
        direction: DropZoneDirectionType,
    ) => void
}
