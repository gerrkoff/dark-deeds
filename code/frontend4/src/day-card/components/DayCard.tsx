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
import { useDayCardHeaderMenu } from '../hooks/useDayCardHeaderMenu'
import { DayCardHeaderMenu } from './DayCardHeaderMenu'
import styles from './DayCard.module.css'

interface Props {
    dayCardModel: DayCardModel
    isDebug: boolean
    isRoutineShown: boolean
    saveTasks: (tasks: TaskModel[]) => void
    onRoutineToggle: (date: Date) => void
}

function DayCard({
    dayCardModel,
    isDebug,
    isRoutineShown,
    saveTasks,
    onRoutineToggle,
}: Props) {
    const cardRef = useRef<HTMLDivElement>(null)

    const {
        taskEditModalContext,
        openTaskEditModal,
        openTaskEditModalForDate,
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

    const {
        headerMenuContext,
        openHeaderMenu,
        closeHeaderMenu,
        onAddTaskForDate,
    } = useDayCardHeaderMenu({
        containerRef: cardRef,
        openTaskEditModalForDate,
    })

    return (
        <>
            <Card elementRef={cardRef} className={styles.card}>
                <DayCardHeader
                    dayCardModel={dayCardModel}
                    isHighlighted={headerMenuContext !== null}
                    isRoutineShown={isRoutineShown}
                    onOpenHeaderMenu={openHeaderMenu}
                    onRoutineToggle={onRoutineToggle}
                />
                <hr className="mt-0 mb-0" />
                <DayCardList
                    date={dayCardModel.date}
                    tasks={dayCardModel.tasks}
                    isDebug={isDebug}
                    isRoutineShown={isRoutineShown}
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

                {headerMenuContext && (
                    <DayCardHeaderMenu
                        context={headerMenuContext}
                        onClose={closeHeaderMenu}
                        onAddTaskForDate={onAddTaskForDate}
                    />
                )}
            </Card>
            <EditTaskModal
                context={taskEditModalContext}
                onClose={closeTaskEditModal}
                onSave={saveAndCloseTaskEditModal}
            />
        </>
    )
}

export { DayCard }
