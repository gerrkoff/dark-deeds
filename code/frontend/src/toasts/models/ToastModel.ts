export interface ToastModel {
    id: string
    type: '' | 'info' | 'success' | 'primary'
    text: string
    category: '' | 'task-save-failed'
    counter: number
}
