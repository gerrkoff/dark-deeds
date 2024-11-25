import { TaskModel } from '../../tasks/models/TaskModel'

export interface DayCardItemMenuContext {
    task: TaskModel
    position: { x: number; y: number }
    anchorElement: HTMLElement
}
