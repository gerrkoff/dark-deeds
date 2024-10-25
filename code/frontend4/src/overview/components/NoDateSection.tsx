import { useCallback, useRef } from 'react'
import { Card } from '../../common/components/Card'
import { DayCardList } from '../../day-card/components/DayCardList'
import { TaskModel } from '../../tasks/models/TaskModel'
import { useDayCardItemMenu } from '../../day-card/hooks/useDayCardItemMenu'
import { DayCardItemMenu } from '../../day-card/components/DayCardItemMenu'
import { useChangeHandlers } from '../../tasks/hooks/useChangeHandlers'
import { EditTaskModal } from '../../edit-task/EditTaskModal'
import { useEditTaskModal } from '../../edit-task/hooks/useEditTaskModal'

interface Props {
    tasks: TaskModel[]
    saveTasks: (tasks: TaskModel[]) => void
}

function NoDateSection({ tasks, saveTasks }: Props) {
    const cardRef = useRef<HTMLDivElement>(null)

    const { itemMenuContext, openItemMenu, closeItemMenu } = useDayCardItemMenu(
        {
            containerRef: cardRef,
        },
    )

    const { taskEditModalContext, openTaskEditModal, closeTaskEditModal } =
        useEditTaskModal()

    const saveTaskAndCloseModal = useCallback(
        (tasks: TaskModel[]) => {
            saveTasks(tasks)
            closeTaskEditModal()
        },
        [closeTaskEditModal, saveTasks],
    )

    const saveTaskAndCloseMenu = useCallback(
        (tasks: TaskModel[]) => {
            saveTasks(tasks)
            closeItemMenu()
        },
        [closeItemMenu, saveTasks],
    )

    const { toggleTaskCompleted, deleteTask } = useChangeHandlers({
        saveTasks: saveTaskAndCloseMenu,
    })

    const editTask = useCallback(
        (task: TaskModel) => {
            closeItemMenu()
            openTaskEditModal(task)
        },
        [closeItemMenu, openTaskEditModal],
    )

    return (
        <Card elementRef={cardRef} style={{ fontSize: '0.8rem' }}>
            <DayCardList tasks={tasks} onTaskContextMenu={openItemMenu} />

            {itemMenuContext && (
                <DayCardItemMenu
                    task={itemMenuContext.task}
                    position={itemMenuContext.position}
                    onClose={closeItemMenu}
                    onToggleCompleted={toggleTaskCompleted}
                    onDelete={deleteTask}
                    onEdit={editTask}
                />
            )}

            <EditTaskModal
                isShown={taskEditModalContext.isShown}
                updatedTask={taskEditModalContext.task}
                onClose={closeTaskEditModal}
                onSave={saveTaskAndCloseModal}
            />
        </Card>
    )
}

export { NoDateSection }
