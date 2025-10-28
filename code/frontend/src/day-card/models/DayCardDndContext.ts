import { TaskModel } from '../../tasks/models/TaskModel'

export const dropZoneBottomIdPrefix = 'bottom-dropzone'
export type DropZoneIdType = string
export type DropZoneDirectionType = 'above' | 'below'

export interface DayCardItemDndContext {
    handleItemDragStart: (item: TaskModel) => void
    handleItemDragOver: (e: DragEvent, dropZoneId: DropZoneIdType, direction: DropZoneDirectionType) => void
    handleItemDrop: (e: DragEvent, dropZoneId: DropZoneIdType, direction: DropZoneDirectionType) => void
    bottomDropZoneId: string
}
