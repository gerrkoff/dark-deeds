export interface ToastModel {
    id: string
    type: '' | 'info' | 'success' | 'primary'
    text: string
    category: '' | 'task-save-failed' | 'task-save-conflict' | 'session-expired'
    counter: number
    autoDismissMs: number | null
}
