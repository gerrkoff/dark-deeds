import clsx from 'clsx'
import { useEffect, useState } from 'react'
import { isTouchDevice } from '../utils/isTouchDevice'
import { isKeyEsc } from '../utils/keys'

interface Props {
    isShown: boolean
    isSaveEnabled: boolean
    onClose: () => void
    onCleanup: () => void
    onSave: () => void
    autoFocusInputRef?: React.RefObject<HTMLInputElement>
    children: React.ReactNode
}

const isStartAnimationEnabled = !isTouchDevice()

function ModalContainer({
    isShown,
    isSaveEnabled,
    onClose,
    onCleanup,
    onSave,
    autoFocusInputRef,
    children,
}: Props) {
    const [visible, setVisible] = useState(!isStartAnimationEnabled)
    const [show, setShow] = useState(!isStartAnimationEnabled)

    useEffect(() => {
        if (isShown) {
            setVisible(true)
            setTimeout(() => setShow(true), 16)
            document.body.style.overflow = 'hidden'
        } else {
            setShow(false)
            setTimeout(() => {
                setVisible(false)
                onCleanup()
            }, 150)
            document.body.style.overflow = ''
        }
    }, [isShown, onCleanup])

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault()
        onSave()
    }

    const handleBackdropClick = (e: React.MouseEvent<HTMLDivElement>) => {
        if (e.target === e.currentTarget) {
            onClose()
        }
    }

    useEffect(() => {
        const handleTouchMove = (e: TouchEvent) => {
            e.preventDefault()
        }

        document.addEventListener('touchmove', handleTouchMove, {
            passive: false,
        })

        return () => {
            document.removeEventListener('touchmove', handleTouchMove)
        }
    }, [])

    const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (isKeyEsc(e)) {
            onClose()
        }
    }

    useEffect(() => {
        setTimeout(() => autoFocusInputRef?.current?.focus(), 16)
    }, [autoFocusInputRef])

    return (
        <div
            className={clsx(
                'modal fade',
                { show },
                { 'd-block': visible },
                { 'd-none': !visible },
            )}
            style={{ backgroundColor: 'rgba(0, 0, 0, 0.5)' }}
            tabIndex={-1}
            aria-label="Edit task"
            onClick={handleBackdropClick}
            onKeyDown={handleKeyDown}
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

export { ModalContainer }
