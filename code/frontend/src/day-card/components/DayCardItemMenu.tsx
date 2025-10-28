import { useState } from 'react'
import { FloatingPanel } from '../../common/components/FloatingPanel'
import { IconCheck } from '../../common/icons/IconCheck'
import { IconPencil } from '../../common/icons/IconPencil'
import { IconX } from '../../common/icons/IconX'
import { IconArrowRight } from '../../common/icons/IconArrowRight'
import { TaskModel } from '../../tasks/models/TaskModel'
import { DayCardItemMenuContext } from '../models/DayCardItemMenuContext'
import clsx from 'clsx'

interface Props {
    context: DayCardItemMenuContext
    onClose: () => void
    onEdit: (task: TaskModel) => void
    onDelete: (task: TaskModel) => void
    onToggleCompleted: (task: TaskModel) => void
    onFollowUp: (task: TaskModel) => void
}

function DayCardItemMenu({
    context: { task, position, anchorElement },
    onClose,
    onDelete,
    onEdit,
    onToggleCompleted,
    onFollowUp,
}: Props) {
    const [isDeletePending, setIsDeletePending] = useState(false)

    const toggleTaskCompleted = () => onToggleCompleted(task)

    const editTask = () => onEdit(task)

    const followUpTask = () => onFollowUp(task)

    const deleteTask = () => {
        if (isDeletePending) {
            onDelete(task)
        }
        setIsDeletePending(true)
    }

    const toggleCompletedLabel = task.completed ? 'Uncomplete' : 'Complete'

    return (
        <FloatingPanel position={position} anchorElement={anchorElement} onClose={onClose}>
            <div className="list-group">
                <button
                    type="button"
                    className="list-group-item list-group-item-action d-flex align-items-center"
                    onClick={toggleTaskCompleted}
                >
                    <IconCheck style={{ minWidth: '20px' }} />
                    {toggleCompletedLabel}
                </button>
                <button
                    type="button"
                    className="list-group-item list-group-item-action d-flex align-items-center"
                    onClick={followUpTask}
                >
                    <IconArrowRight style={{ minWidth: '20px' }} />
                    Follow Up
                </button>
                <button
                    type="button"
                    className="list-group-item list-group-item-action d-flex align-items-center"
                    onClick={editTask}
                >
                    <IconPencil size={10} style={{ minWidth: '20px' }} />
                    Edit
                </button>
                <button
                    type="button"
                    className={clsx('list-group-item list-group-item-action d-flex align-items-center', {
                        'bg-danger': isDeletePending,
                    })}
                    onClick={deleteTask}
                    data-test-id="btn-delete-item"
                >
                    <IconX style={{ minWidth: '20px' }} />
                    Delete
                </button>
            </div>
        </FloatingPanel>
    )
}

export { DayCardItemMenu }
