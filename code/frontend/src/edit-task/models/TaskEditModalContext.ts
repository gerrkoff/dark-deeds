import { ModalContainerContext } from '../../common/models/ModalContainerContext'
import { TaskModel } from '../../tasks/models/TaskModel'

export interface TaskEditModalContext extends ModalContainerContext {
    task: TaskModel | null
    date: Date | null
}
