import * as c from '../constants'

export function openModalConfirm(content: React.ReactNode, action: () => void, header?: string, headerIcon?: string): c.IModalConfirmOpen {
    return { type: c.MODALCONFIRM_OPEN, content, action, header, headerIcon }
}

export function closeModalConfirm(): c.IModalConfirmClose {
    return { type: c.MODALCONFIRM_CLOSE }
}
