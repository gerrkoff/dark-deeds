import clsx from 'clsx'
import { ToastModel } from '../models/ToastModel'
import styles from './Toast.module.css'

interface Props {
    toast: ToastModel
    onClose: (id: string) => void
}

function Toast({ toast, onClose }: Props) {
    return (
        <div
            className={clsx('toast show align-items-center border-0', styles.show, {
                'bg-primary': toast.type === 'primary',
                'bg-success': toast.type === 'success',
                'bg-info': toast.type === 'info',
            })}
        >
            <div className="d-flex">
                <div className="toast-body">
                    {toast.text}
                    {toast.counter ? ` [${toast.counter}]` : ''}
                </div>
                <button
                    type="button"
                    className="btn-close btn-close-white me-2 m-auto"
                    aria-label="Close"
                    onClick={() => onClose(toast.id)}
                ></button>
            </div>
        </div>
    )
}

export { Toast }
