import { ITasksState } from './tasks'
import { IEditTaskState } from './editTask'
import { IModalConfirmState } from './modalConfirm'
import { ILoginState } from './login'
import { IGeneralState } from './general'
import { ITelegramIntegration } from './telegramIntegration'

export interface IAppState {
    router: any
    login: ILoginState
    tasks: ITasksState
    editTask: IEditTaskState
    modalConfirm: IModalConfirmState
    general: IGeneralState
    telegramIntegration: ITelegramIntegration
}

export * from './tasks'
export * from './editTask'
export * from './modalConfirm'
export * from './login'
export * from './general'
export * from './telegramIntegration'
