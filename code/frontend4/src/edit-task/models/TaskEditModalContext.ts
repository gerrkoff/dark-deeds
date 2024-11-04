import { TaskModel } from '../../tasks/models/TaskModel'

export interface TaskEditModalContext {
    isShown: boolean
    task: TaskModel | null
    date: Date | null
    close: () => void
    cleanup: () => void
}
