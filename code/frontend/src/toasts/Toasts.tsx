import { useCallback } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { removeToast } from './redux/toasts-slice'
import { Toast } from './components/Toast'

function Toasts() {
    const dispatch = useAppDispatch()

    const { toasts } = useAppSelector(state => state.toasts)

    const onToastClose = useCallback(
        (id: string) => {
            dispatch(removeToast(id))
        },
        [dispatch],
    )

    return (
        <div
            className="position-fixed top-0 end-0 p-3 toast-container"
            style={{ zIndex: 11 }}
        >
            {toasts.map(x => (
                <Toast key={x.id} toast={x} onClose={onToastClose} />
            ))}
        </div>
    )
}

export { Toasts }
