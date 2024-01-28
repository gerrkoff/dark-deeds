import { toast, ToastOptions } from 'react-toastify'

const defaultOptions: ToastOptions = {
    progressClassName: 'toast-progress',
    closeButton: false,
    autoClose: 5000,
    closeOnClick: true,
    draggable: true,
}

export class ToastService {
    public readonly types = toast.TYPE

    public success(msg: string, options: ToastOptions = {}): number {
        return toast.success(msg, {
            className: 'toast-success',
            ...defaultOptions,
            ...options,
        })
    }

    public error(msg: string, options: ToastOptions = {}): number {
        return toast.error(msg, {
            className: 'toast-error',
            ...defaultOptions,
            ...options,
        })
    }

    public warn(msg: string, options: ToastOptions = {}): number {
        return toast.warn(msg, {
            className: 'toast-warn',
            ...defaultOptions,
            ...options,
        })
    }

    public info(msg: string, options: ToastOptions = {}): number {
        return toast.info(msg, {
            className: 'toast-info',
            ...defaultOptions,
            ...options,
        })
    }

    public errorProcess(process: string, options = {}): number {
        return this.error(`Error occured while ${process}`, options)
    }

    public dismiss(toastId: number | string) {
        toast.dismiss(toastId as any)
    }

    public update(
        toastId: number | string,
        msg: string,
        options: ToastOptions = {}
    ) {
        toast.update(toastId as any, {
            render: msg,
            ...defaultOptions,
            ...options,
        })
    }

    public isActive(toastId: number | string): boolean {
        return toast.isActive(toastId as any)
    }
}

export const toastService = new ToastService()
