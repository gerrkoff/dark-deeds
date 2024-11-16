import { useCallback, useEffect, useRef } from 'react'
import { EditRecurrenceModalContext } from '../models/EditRecurrenceModalContext'
import { PlannedRecurrenceModel } from '../models/PlannedRecurrenceModel'
import { ModalContainer } from '../../common/components/ModalContainer'

interface Props {
    context: EditRecurrenceModalContext
    onSave: (recurrence: PlannedRecurrenceModel) => void
}

function EditRecurrenceModal({ context, onSave }: Props) {
    const inputRef = useRef<HTMLInputElement>(null)

    const { close, cleanup } = context

    const handleSave = useCallback(() => {
        if (context.recurrence) {
            onSave(context.recurrence)
        }
        console.log('Save')
    }, [context.recurrence, onSave])

    useEffect(() => {
        setTimeout(() => inputRef.current?.focus(), 16)
    }, [])

    const label = 'test'
    const task = 'test'

    return (
        <ModalContainer
            isShown={context.isShown}
            onClose={close}
            onCleanup={cleanup}
            onSave={handleSave}
            isSaveEnabled={false}
        >
            <div className="form-floating mb-3">
                <input
                    autoFocus
                    ref={inputRef}
                    type="text"
                    className="form-control"
                    id="taskInput"
                    placeholder={label}
                    value={task}
                />
                <label htmlFor="taskInput">{label}</label>
            </div>
        </ModalContainer>
    )
}

export { EditRecurrenceModal }
