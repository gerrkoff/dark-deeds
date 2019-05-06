import { toast, ToastOptions } from 'react-toastify'

const defaultOptions: ToastOptions = {
    progressClassName: 'toast-progress'
}

const service = {
    success(msg: string, options: ToastOptions = {}): number {
        return toast.success(msg, {
            className: 'toast-success',
            ...defaultOptions,
            ...options
        })
    },
    error(msg: string, options: ToastOptions = {}): number {
        return toast.error(msg, {
            className: 'toast-error',
            ...defaultOptions,
            ...options
        })
    },
    warn(msg: string, options: ToastOptions = {}): number {
        return toast.warn(msg, {
            className: 'toast-warn',
            ...defaultOptions,
            ...options
        })
    },
    info(msg: string, options: ToastOptions = {}): number {
        return toast.info(msg, {
            className: 'toast-info',
            ...defaultOptions,
            ...options
        })
    },

    errorProcess(process: string, options = {}): number {
        return this.error(`Error occured while ${process}`, options)
    },

    dismiss(toastId: number) {
        toast.dismiss(toastId)
    }
}

export { service as ToastService }
