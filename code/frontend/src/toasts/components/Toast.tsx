import clsx from 'clsx'
import { useEffect } from 'react'
import { ToastModel } from '../models/ToastModel'
import styles from './Toast.module.css'

interface Props {
    toast: ToastModel
    onClose: (id: string) => void
}

function Toast({ toast, onClose }: Props) {
    const { id, autoDismissMs, counter } = toast

    // Restart the auto-dismiss timer whenever the toast is re-triggered (counter bumps when a
    // same-category toast is stacked), so an active toast does not vanish while related events
    // keep arriving.
    useEffect(() => {
        if (autoDismissMs === null) {
            return
        }

        const timeout = setTimeout(() => onClose(id), autoDismissMs)
        return () => clearTimeout(timeout)
    }, [id, autoDismissMs, onClose, counter])

    return (
        <div
            className={clsx('toast show align-items-center border-0', styles.toast, styles.show, {
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
            {autoDismissMs !== null && (
                <div key={counter} className={styles.progress} style={{ animationDuration: `${autoDismissMs}ms` }} />
            )}
        </div>
    )
}

export { Toast }
