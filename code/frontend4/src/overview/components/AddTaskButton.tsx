import { useCallback, useState } from 'react'
import { IconPlusLg } from '../../common/icons/IconPlusLg'
import { EditTaskModal } from '../../ui/components/edit-task-modal/EditTaskModal'
import { TaskEntity } from '../../common/models/TaskEntity'

function AddTaskButton() {
    const [isEditTaskModalShown, setIsEditTaskModalShown] = useState(false)

    const handleClose = useCallback(() => setIsEditTaskModalShown(false), [])

    const handleSave = useCallback((tasks: TaskEntity[]) => {
        console.log('save', tasks)
        setIsEditTaskModalShown(false)
    }, [])

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
            />
        </>
    )
}

export { AddTaskButton }
