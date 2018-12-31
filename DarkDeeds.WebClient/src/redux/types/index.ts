import { ITasksState } from './tasks'
import { IEditTaskState } from './editTask'
import { IModalConfirmState } from './modalConfirm'
import { ILoginState } from './login'
import { IGeneralState } from './general'

export interface IAppState {
    router: any
    login: ILoginState
    tasks: ITasksState
    editTask: IEditTaskState
    modalConfirm: IModalConfirmState
    general: IGeneralState
}

export * from './tasks'
export * from './editTask'
export * from './modalConfirm'
export * from './login'
export * from './general'
