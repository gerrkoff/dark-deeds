import clsx from 'clsx'
import { useEffect, useState } from 'react'

interface Props {
    isShown: boolean
    isSaveEnabled: boolean
    onClose: () => void
    onSave: () => void
    children: React.ReactNode
}

function EditTaskModalContainer({
    isShown,
    isSaveEnabled,
    onClose,
    onSave,
    children,
}: Props) {
    const [visible, setVisible] = useState(false)
    const [show, setShow] = useState(false)

    useEffect(() => {
        if (isShown) {
            setVisible(true)
            setTimeout(() => setShow(true), 16)
            document.body.style.overflow = 'hidden'
        } else {
            setShow(false)
            setTimeout(() => setVisible(false), 150)
            document.body.style.overflow = ''
        }
    }, [isShown])

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault()
        onSave()
    }

    const handleBackdropClick = (e: React.MouseEvent<HTMLDivElement>) => {
        if (e.target === e.currentTarget) {
            onClose()
        }
    }

    return (
        <div
            className={clsx(
                'modal fade',
                { show },
                { 'd-block': visible },
                { 'd-none': !visible },
            )}
            style={{ backgroundColor: 'rgba(0, 0, 0, 0.5)' }}
            id="exampleModal"
            tabIndex={-1}
            aria-label="Edit task"
            onClick={handleBackdropClick}
        >
            <div className="modal-dialog">
                <div className="modal-content">
                    <form onSubmit={handleSubmit}>
                        <div className="modal-body">{children}</div>
                        <div className="modal-footer">
                            <button
                                type="button"
                                className="btn btn-secondary"
                                onClick={onClose}
                            >
                                Close
                            </button>
                            <button
                                type="submit"
                                className="btn btn-primary"
                                disabled={!isSaveEnabled}
                            >
                                Save changes
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    )
}

export { EditTaskModalContainer }
