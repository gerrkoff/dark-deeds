import { toast } from 'react-toastify'

const service = {
    success(msg: string, options = {}): number {
        return toast.success(msg, {
            ...options,
            className: 'toast-success',
            progressClassName: 'toast-progress'
        })
    },
    error(msg: string, options = {}): number {
        return toast.error(msg, {
            ...options,
            className: 'toast-error',
            progressClassName: 'toast-progress'
        })
    },
    warn(msg: string, options = {}): number {
        return toast.warn(msg, {
            ...options,
            className: 'toast-warn',
            progressClassName: 'toast-progress'
        })
    },
    info(msg: string, options = {}): number {
        return toast.info(msg, {
            ...options,
            className: 'toast-info',
            progressClassName: 'toast-progress'
        })
    },
    errorCommon(err: any): number {
        return this.error('Error!')
    }
}

export { service as ToastHelper }
