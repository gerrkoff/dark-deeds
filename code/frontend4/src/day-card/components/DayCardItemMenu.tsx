import { FloatingPanel } from '../../common/components/FloatingPanel'
import { TaskModel } from '../../tasks/models/TaskModel'

interface Props {
    task: TaskModel
    position: { x: number; y: number }
    onClose: () => void
    onEdit: (task: TaskModel) => void
    onDelete: (task: TaskModel) => void
    onToggleCompleted: (task: TaskModel) => void
}

function DayCardItemMenu({
    task,
    position,
    onClose,
    onDelete,
    onEdit,
    onToggleCompleted,
}: Props) {
    const toggleTaskCompleted = () => onToggleCompleted(task)

    const editTask = () => onEdit(task)

    const deleteTask = () => onDelete(task)

    return (
        <FloatingPanel position={position} onClose={onClose}>
            <div className="list-group">
                <button
                    type="button"
                    className="list-group-item list-group-item-action"
                    onClick={toggleTaskCompleted}
                >
                    Complete
                </button>
                <button
                    type="button"
                    className="list-group-item list-group-item-action"
                    onClick={editTask}
                >
                    Edit
                </button>
                <button
                    type="button"
                    className="list-group-item list-group-item-action"
                    onClick={deleteTask}
                >
                    Delete
                </button>
            </div>
        </FloatingPanel>
    )
}

export { DayCardItemMenu }
