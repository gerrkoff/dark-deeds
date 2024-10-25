import { useCallback } from 'react'
import { IconPlusLg } from '../../common/icons/IconPlusLg'
import { EditTaskModal } from '../../edit-task/EditTaskModal'
import { TaskModel } from '../../tasks/models/TaskModel'
import { useEditTaskModal } from '../../edit-task/hooks/useEditTaskModal'

interface Props {
    saveTasks: (tasks: TaskModel[]) => void
}

function AddTaskButton({ saveTasks }: Props) {
    const { taskEditModalContext, openTaskEditModal, closeTaskEditModal } =
        useEditTaskModal()

    const handleSave = useCallback(
        (tasks: TaskModel[]) => {
            saveTasks(tasks)
            closeTaskEditModal()
        },
        [closeTaskEditModal, saveTasks],
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
                    onClick={() => openTaskEditModal(null)}
                >
                    <IconPlusLg />
                </button>
            </div>

            <EditTaskModal
                isShown={taskEditModalContext.isShown}
                updatedTask={taskEditModalContext.task}
                onClose={closeTaskEditModal}
                onSave={handleSave}
            />
        </>
    )
}

export { AddTaskButton }
