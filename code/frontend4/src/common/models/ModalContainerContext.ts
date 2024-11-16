export interface ModalContainerContext {
    isShown: boolean
    close: () => void
    cleanup: () => void
}
