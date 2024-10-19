import clsx from 'clsx'
import { useEffect, useState } from 'react'

interface Props {
    isShown: boolean
    onClose: () => void
    onSave: () => void
    children: React.ReactNode
}

function EditTaskModalContainer({ isShown, onClose, onSave, children }: Props) {
    const [show, setShow] = useState(false)

    useEffect(() => {
        if (isShown) {
            setTimeout(() => setShow(true), 16)
        } else {
            setShow(false)
        }
    }, [isShown])

    return (
        <div
            className={clsx('modal fade', { show })}
            style={{
                backgroundColor: 'rgba(0, 0, 0, 0.5)',
                display: isShown ? 'block' : 'none',
            }}
            id="exampleModal"
            tabIndex={-1}
            aria-label="Edit task"
        >
            <div className="modal-dialog">
                <div className="modal-content">
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
                            type="button"
                            className="btn btn-primary"
                            onClick={onSave}
                        >
                            Save changes
                        </button>
                    </div>
                </div>
            </div>
        </div>
    )
}

export { EditTaskModalContainer }
