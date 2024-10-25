import { useRef } from 'react'
import { DayCardModel } from '../models/DayCardModel'
import { useDayCardItemMenu } from '../hooks/useDayCardItemMenu'
import { Card } from '../../common/components/Card'
import { DayCardHeader } from './DayCardHeader'
import { DayCardList } from './DayCardList'
import { DayCardItemMenu } from './DayCardItemMenu'
import { TaskModel } from '../../tasks/models/TaskModel'
import { EditTaskModal } from '../../edit-task/EditTaskModal'
import { useEditTaskModal } from '../../edit-task/hooks/useEditTaskModal'

interface Props {
    dayCardModel: DayCardModel
    saveTasks: (tasks: TaskModel[]) => void
}

function DayCard({ dayCardModel, saveTasks }: Props) {
    const cardRef = useRef<HTMLDivElement>(null)

    const {
        taskEditModalContext,
        openTaskEditModal,
        closeTaskEditModal,
        saveAndCloseTaskEditModal,
    } = useEditTaskModal({ saveTasks })

    const {
        itemMenuContext,
        openItemMenu,
        closeItemMenu,
        deleteTask,
        editTask,
        toggleTaskCompleted,
    } = useDayCardItemMenu({
        containerRef: cardRef,
        saveTasks,
        openTaskEditModal,
    })

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
                context={taskEditModalContext}
                onClose={closeTaskEditModal}
                onSave={saveAndCloseTaskEditModal}
            />
        </Card>
    )
}

export { DayCard }
