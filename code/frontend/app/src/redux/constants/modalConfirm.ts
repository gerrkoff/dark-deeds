export const MODALCONFIRM_OPEN = 'MODALCONFIRM_OPEN'
export type MODALCONFIRM_OPEN = typeof MODALCONFIRM_OPEN
export interface IModalConfirmOpen {
    type: MODALCONFIRM_OPEN
    content: React.ReactNode
    action: () => void
    header?: string
    headerIcon?: string
}

export const MODALCONFIRM_CLOSE = 'MODALCONFIRM_CLOSE'
export type MODALCONFIRM_CLOSE = typeof MODALCONFIRM_CLOSE
export interface IModalConfirmClose {
    type: MODALCONFIRM_CLOSE
}

export type ModalConfirmAction = IModalConfirmOpen | IModalConfirmClose
