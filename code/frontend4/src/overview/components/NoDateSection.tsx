import { useRef } from 'react'
import { Card } from '../../common/components/Card'
import { DayCardList } from '../../day-card/components/DayCardList'
import { TaskModel } from '../../tasks/models/TaskModel'
import { useDayCardItemMenu } from '../../day-card/hooks/useDayCardItemMenu'
import { DayCardItemMenu } from '../../day-card/components/DayCardItemMenu'
import { EditTaskModal } from '../../edit-task/EditTaskModal'
import { useEditTaskModal } from '../../edit-task/hooks/useEditTaskModal'

interface Props {
    tasks: TaskModel[]
    isDebug: boolean
    saveTasks: (tasks: TaskModel[]) => void
}

function NoDateSection({ tasks, isDebug, saveTasks }: Props) {
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
            style={{ fontSize: '0.8rem', minHeight: '70px' }}
        >
            <DayCardList
                date={null}
                tasks={tasks}
                isDebug={isDebug}
                isRoutineShown={true}
                openedMenuTaskUid={itemMenuContext?.task.uid ?? null}
                onOpenTaskMenu={openItemMenu}
                onSaveTasks={saveTasks}
            />

            {itemMenuContext && (
                <DayCardItemMenu
                    context={itemMenuContext}
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

export { NoDateSection }
