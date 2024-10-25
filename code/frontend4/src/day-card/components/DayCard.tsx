import { useCallback, useRef } from 'react'
import { DayCardModel } from '../models/DayCardModel'
import { useDayCardItemMenu } from '../hooks/useDayCardItemMenu'
import { Card } from '../../common/components/Card'
import { DayCardHeader } from './DayCardHeader'
import { DayCardList } from './DayCardList'
import { DayCardItemMenu } from './DayCardItemMenu'
import { TaskModel } from '../../tasks/models/TaskModel'
import { useChangeHandlers } from '../../tasks/hooks/useChangeHandlers'
import { EditTaskModal } from '../../edit-task/EditTaskModal'
import { useEditTaskModal } from '../../edit-task/hooks/useEditTaskModal'

interface Props {
    dayCardModel: DayCardModel
    saveTasks: (tasks: TaskModel[]) => void
}

function DayCard({ dayCardModel, saveTasks }: Props) {
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
        <Card
            elementRef={cardRef}
            style={{
                minWidth: '160px',
                minHeight: '100px',
                height: '100%',
                fontSize: '0.8rem',
            }}
        >
            <DayCardHeader dayCardModel={dayCardModel} />
            <hr className="mt-0 mb-0" />
            <DayCardList
                tasks={dayCardModel.tasks}
                onTaskContextMenu={openItemMenu}
            />

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

export { DayCard }
