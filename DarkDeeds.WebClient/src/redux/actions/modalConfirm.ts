import * as actions from '../constants'

export function openModalConfirm(content: React.ReactNode, action: () => void, header?: string, headerIcon?: string): actions.IModalConfirmOpen {
    return { type: actions.MODALCONFIRM_OPEN, content, action, header, headerIcon }
}

export function closeModalConfirm(): actions.IModalConfirmClose {
    return { type: actions.MODALCONFIRM_CLOSE }
}
