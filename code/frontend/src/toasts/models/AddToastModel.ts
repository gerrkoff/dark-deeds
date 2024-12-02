export interface AddToastModel {
    type?: 'info' | 'success' | 'primary'
    text: string
    category?: 'task-save-failed'
}
