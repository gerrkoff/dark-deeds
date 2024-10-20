import { useCallback, useState } from 'react'
import { IconPlusLg } from '../../common/icons/IconPlusLg'
import { EditTaskModal } from '../../edit-task/EditTaskModal'
import { TaskModel } from '../../tasks/models/TaskModel'

interface Props {
    onAddTasks: (tasks: TaskModel[]) => void
}

function AddTaskButton({ onAddTasks }: Props) {
    const [isEditTaskModalShown, setIsEditTaskModalShown] = useState(false)

    const handleClose = useCallback(() => setIsEditTaskModalShown(false), [])

    const handleSave = useCallback(
        (tasks: TaskModel[]) => {
            onAddTasks(tasks)
            setIsEditTaskModalShown(false)
        },
        [onAddTasks],
    )

    return (
        <>
            <div
                className="position-fixed bottom-0 end-0 shadow"
                style={{
                    marginBottom: '76px',
                    marginRight: '20px',
                }}
            >
                <button
                    className="btn btn-primary d-flex align-items-center"
                    style={{
                        minHeight: '42px',
                        minWidth: '42px',
                        borderRadius: '50%',
                    }}
                    onClick={() => setIsEditTaskModalShown(true)}
                >
                    <IconPlusLg />
                </button>
            </div>

            <EditTaskModal
                isShown={isEditTaskModalShown}
                onClose={handleClose}
                onSave={handleSave}
                updatedTask={null}
            />
        </>
    )
}

export { AddTaskButton }
