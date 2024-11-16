import { useCallback, useEffect, useRef, useState } from 'react'
import { EditRecurrenceModalContext } from '../models/EditRecurrenceModalContext'
import { PlannedRecurrenceModel } from '../models/PlannedRecurrenceModel'
import { ModalContainer } from '../../common/components/ModalContainer'

interface Props {
    context: EditRecurrenceModalContext
    onSave: (recurrence: PlannedRecurrenceModel) => void
}

const taskLabel = 'Task...'

function EditRecurrenceModal({ context, onSave }: Props) {
    const inputRef = useRef<HTMLInputElement>(null)

    const { recurrence } = context

    const [task, setTask] = useState(recurrence?.task ?? '')

    const handleSave = useCallback(() => {
        if (context.recurrence) {
            onSave(context.recurrence)
        }
        console.log('Save')
    }, [context.recurrence, onSave])

    useEffect(() => {
        setTimeout(() => inputRef.current?.focus(), 16)
    }, [])

    return (
        <ModalContainer
            context={context}
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
                    placeholder={taskLabel}
                    value={task}
                    onChange={e => setTask(e.target.value)}
                />
                <label htmlFor="taskInput">{taskLabel}</label>
            </div>
        </ModalContainer>
    )
}

export { EditRecurrenceModal }
