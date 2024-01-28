import { IEditTaskState } from 'redux/types/editTask'
import { IGeneralState } from 'redux/types/general'
import { ILoginState } from 'redux/types/login'
import { IModalConfirmState } from 'redux/types/modalConfirm'
import { IRecurrencesViewState } from 'redux/types/recurrencesView'
import { ISettings } from 'redux/types/settings'
import { ITasksState } from 'redux/types/tasks'
import { ITelegramIntegration } from 'redux/types/telegramIntegration'

export interface IAppState {
    router: any
    login: ILoginState
    tasks: ITasksState
    editTask: IEditTaskState
    modalConfirm: IModalConfirmState
    general: IGeneralState
    telegramIntegration: ITelegramIntegration
    settings: ISettings
    recurrencesView: IRecurrencesViewState
}

export * from 'redux/types/editTask'
export * from 'redux/types/general'
export * from 'redux/types/login'
export * from 'redux/types/modalConfirm'
export * from 'redux/types/recurrencesView'
export * from 'redux/types/settings'
export * from 'redux/types/tasks'
export * from 'redux/types/telegramIntegration'
