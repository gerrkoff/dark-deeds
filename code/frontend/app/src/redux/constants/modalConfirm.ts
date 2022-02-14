export const MODALCONFIRM_OPEN = 'MODALCONFIRM_OPEN'
export interface IModalConfirmOpen {
    type: typeof MODALCONFIRM_OPEN
    content: React.ReactNode
    action: () => void
    header?: string
    headerIcon?: string
}

export const MODALCONFIRM_CLOSE = 'MODALCONFIRM_CLOSE'
export interface IModalConfirmClose {
    type: typeof MODALCONFIRM_CLOSE
}

export type ModalConfirmAction = IModalConfirmOpen | IModalConfirmClose
