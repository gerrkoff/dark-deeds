import clsx from 'clsx'
import { useEffect, useRef, useState } from 'react'
import { isTouchDevice } from '../utils/isTouchDevice'
import { isKeyEsc } from '../utils/keys'
import { ModalContainerContext } from '../models/ModalContainerContext'

interface Props {
    context: ModalContainerContext
    isSaveEnabled: boolean
    onSave: () => void
    onDelete?: () => void
    autoFocusInputRef?: React.RefObject<HTMLInputElement>
    children: React.ReactNode
}

const isStartAnimationEnabled = !isTouchDevice()
const isMobile = isTouchDevice()

function ModalContainer({
    context: { isShown, close, cleanup },
    isSaveEnabled,
    onSave,
    onDelete,
    autoFocusInputRef,
    children,
}: Props) {
    const containerRef = useRef<HTMLDivElement>(null)
    const [visible, setVisible] = useState(!isStartAnimationEnabled)
    const [show, setShow] = useState(!isStartAnimationEnabled)

    useEffect(() => {
        if (!isMobile) {
            return
        }

        const appContainer = document.getElementById('app-container')
        if (containerRef.current && appContainer) {
            containerRef.current.style.height = `${appContainer?.offsetHeight}px`
        }
    }, [])

    useEffect(() => {
        if (isShown) {
            setVisible(true)
            setTimeout(() => setShow(true), 16)
            if (!isMobile) {
                document.body.style.overflow = 'hidden'
            }
        } else {
            setShow(false)
            setTimeout(() => {
                setVisible(false)
                cleanup()
            }, 150)
            if (!isMobile) {
                document.body.style.overflow = ''
            }
        }
    }, [isShown, cleanup])

    const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault()
        onSave()
    }

    const handleBackdropClick = (e: React.MouseEvent<HTMLDivElement>) => {
        if (e.target === e.currentTarget) {
            close()
        }
    }

    const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
        if (isKeyEsc(e)) {
            close()
        }
    }

    useEffect(() => {
        setTimeout(() => autoFocusInputRef?.current?.focus(), 16)
    }, [autoFocusInputRef])

    const [isDeletePending, setIsDeletePending] = useState(false)

    const handleDelete = () => {
        if (!onDelete) {
            return
        }

        if (isDeletePending) {
            onDelete()
        }

        setIsDeletePending(true)
    }

    const isDeleteVisible = !!onDelete

    return (
        <div
            ref={containerRef}
            className={clsx('modal fade', { show }, { 'd-block': visible }, { 'd-none': !visible })}
            style={{
                backgroundColor: 'rgba(0, 0, 0, 0.5)',
                position: isMobile ? 'absolute' : undefined,
            }}
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
                            <button type="button" className="btn btn-secondary" onClick={close}>
                                Close
                            </button>
                            {isDeleteVisible && (
                                <button
                                    type="button"
                                    className={clsx('btn', {
                                        'btn-secondary': !isDeletePending,
                                        'btn-danger': isDeletePending,
                                    })}
                                    onClick={handleDelete}
                                >
                                    Delete
                                </button>
                            )}
                            <button type="submit" className="btn btn-primary" disabled={!isSaveEnabled}>
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
