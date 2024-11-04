import { FloatingPanel } from '../../common/components/FloatingPanel'
import { IconCheck } from '../../common/icons/IconCheck'
import { IconPencil } from '../../common/icons/IconPencil'
import { IconX } from '../../common/icons/IconX'
import { TaskModel } from '../../tasks/models/TaskModel'
import { DayCardItemMenuContext } from '../models/DayCardItemMenuContext'

interface Props {
    context: DayCardItemMenuContext
    onClose: () => void
    onEdit: (task: TaskModel) => void
    onDelete: (task: TaskModel) => void
    onToggleCompleted: (task: TaskModel) => void
}

function DayCardItemMenu({
    context: { task, position, anchorElement },
    onClose,
    onDelete,
    onEdit,
    onToggleCompleted,
}: Props) {
    const toggleTaskCompleted = () => onToggleCompleted(task)

    const editTask = () => onEdit(task)

    const deleteTask = () => onDelete(task)

    const toggleCompletedLabel = task.completed ? 'Uncomplete' : 'Complete'

    return (
        <FloatingPanel
            position={position}
            anchorElement={anchorElement}
            onClose={onClose}
        >
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
                    onClick={editTask}
                >
                    <IconPencil size={10} style={{ minWidth: '20px' }} />
                    Edit
                </button>
                <button
                    type="button"
                    className="list-group-item list-group-item-action d-flex align-items-center"
                    onClick={deleteTask}
                >
                    <IconX style={{ minWidth: '20px' }} />
                    Delete
                </button>
            </div>
        </FloatingPanel>
    )
}

export { DayCardItemMenu }
