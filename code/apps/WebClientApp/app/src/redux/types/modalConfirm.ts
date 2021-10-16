export interface IModalConfirmState {
    open: boolean
    header: string
    headerIcon: string
    content: React.ReactNode
    action: () => void
}
