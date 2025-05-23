import { IconPlusLg } from '../../common/icons/IconPlusLg'
import { EditTaskModal } from '../../edit-task/EditTaskModal'
import { TaskModel } from '../../tasks/models/TaskModel'
import { useEditTaskModal } from '../../edit-task/hooks/useEditTaskModal'
import { useEffect } from 'react'
import { isKeyEnter } from '../../common/utils/keys'

interface Props {
    saveTasks: (tasks: TaskModel[]) => void
}

function AddTaskButton({ saveTasks }: Props) {
    const { taskEditModalContext, openTaskEditModal } = useEditTaskModal()

    useEffect(() => {
        const onKeydown = (event: KeyboardEvent) => {
            if (isKeyEnter(event) && (event.ctrlKey || event.metaKey)) {
                openTaskEditModal(null)
            }
        }

        document.addEventListener('keydown', onKeydown)

        return () => {
            document.removeEventListener('keydown', onKeydown)
        }
    }, [openTaskEditModal])

    return (
        <>
            <div
                className="position-fixed bottom-0 end-0"
                style={{
                    marginBottom: '76px',
                    marginRight: '20px',
                }}
            >
                <button
                    data-test-id="btn-add-task"
                    className="btn btn-primary d-flex align-items-center shadow"
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

            {taskEditModalContext && (
                <EditTaskModal
                    context={taskEditModalContext}
                    onSave={saveTasks}
                />
            )}
        </>
    )
}

export { AddTaskButton }
