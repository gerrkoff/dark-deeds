import * as constants from '../constants'

export interface IModalConfirmOpen {
    type: constants.MODALCONFIRM_OPEN
    content: React.ReactNode
    action: () => void
    header?: string
    headerIcon?: string
}

export interface IModalConfirmClose {
    type: constants.MODALCONFIRM_CLOSE
}

export type ModalConfirmAction = IModalConfirmOpen | IModalConfirmClose

export function openModalConfirm(content: React.ReactNode, action: () => void, header?: string, headerIcon?: string): IModalConfirmOpen {
    return { type: constants.MODALCONFIRM_OPEN, content, action, header, headerIcon }
}

export function closeModalConfirm(): IModalConfirmClose {
    return { type: constants.MODALCONFIRM_CLOSE }
}
