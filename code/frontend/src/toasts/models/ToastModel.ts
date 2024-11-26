import { ToastType } from './ToastTypeEnum'

export interface ToastModel {
    id: string
    type?: ToastType
    text: string
}
