import { FloatingPanel } from '../../common/components/FloatingPanel'
import { TaskModel } from '../../tasks/models/TaskModel'

interface Props {
    task: TaskModel
    position: { x: number; y: number }
    onClose: () => void
}

function DayCardItemMenu({ task, position, onClose }: Props) {
    const toggleTaskCompleted = () => {
        console.log('toggleTaskCompleted', task)
    }

    const editTask = () => {
        console.log('editTask', task)
    }

    const deleteTask = () => {
        console.log('deleteTask', task)
    }

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
