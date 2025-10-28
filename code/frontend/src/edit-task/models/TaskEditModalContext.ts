import { ModalContainerContext } from '../../common/models/ModalContainerContext'
import { TaskModel } from '../../tasks/models/TaskModel'

export type TaskEditModalContent = EditTaskContent | NewFromDateContent | NewFromTaskContent | NewContent

export interface TaskEditModalContext extends ModalContainerContext {
    content: TaskEditModalContent
}

export interface EditTaskContent {
    type: 'EDIT'
    task: TaskModel
}

export interface NewFromDateContent {
    type: 'NEW_FROM_DATE'
    date: Date
}

export interface NewFromTaskContent {
    type: 'NEW_FROM_TASK'
    task: TaskModel
}

export interface NewContent {
    type: 'NEW'
}
