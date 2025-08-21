import { useCallback, useRef } from 'react'
import { DayCardModel } from '../models/DayCardModel'
import { Card } from '../../common/components/Card'
import { DayCardHeader } from './DayCardHeader'
import { DayCardList } from './DayCardList'
import { DayCardItemMenu } from './DayCardItemMenu'
import { TaskModel } from '../../tasks/models/TaskModel'
import { EditTaskModal } from '../../edit-task/EditTaskModal'
import { useEditTaskModal } from '../../edit-task/hooks/useEditTaskModal'
import { DayCardHeaderMenu } from './DayCardHeaderMenu'
import styles from './DayCard.module.css'
import { useDayCardMenuItem } from '../hooks/useDayCardMenuItem'
import { useDayCardMenuHeader } from '../hooks/useDayCardMenuHeader'
import { dateService } from '../../common/services/DateService'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'

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
    } = useEditTaskModal()

    const {
        itemMenuContext,
        openItemMenu,
        closeItemMenu,
        deleteTask,
        editTask,
        toggleTaskCompleted,
    } = useDayCardMenuItem({
        containerRef: cardRef,
        saveTasks,
        openTaskEditModal,
    })

    const {
        headerMenuContext,
        openHeaderMenu,
        closeHeaderMenu,
        onAddTaskForDate,
    } = useDayCardMenuHeader({
        containerRef: cardRef,
        openTaskEditModalForDate,
    })

    const isExpired = dayCardModel.date < dateService.today()

    const transformDrop = useCallback(
        (task: TaskModel) => ({
            ...task,
            type:
                task.type === TaskTypeEnum.Weekly
                    ? TaskTypeEnum.Simple
                    : task.type,
            date: dayCardModel.date.valueOf(),
        }),
        [dayCardModel.date],
    )

    return (
        <>
            <Card
                elementRef={cardRef}
                isDimmed={isExpired}
                className={styles.card}
            >
                <DayCardHeader
                    dayCardModel={dayCardModel}
                    isHighlighted={headerMenuContext !== null}
                    isRoutineShown={isRoutineShown}
                    onOpenHeaderMenu={openHeaderMenu}
                    onRoutineToggle={onRoutineToggle}
                />
                <hr className="mt-0 mb-0" />
                <DayCardList
                    tasks={dayCardModel.tasks}
                    isDebug={isDebug}
                    isRoutineShown={isRoutineShown}
                    openedMenuTaskUid={itemMenuContext?.task.uid ?? null}
                    onOpenTaskMenu={openItemMenu}
                    onSaveTasks={saveTasks}
                    onTransformDrop={transformDrop}
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

            {taskEditModalContext && (
                <EditTaskModal
                    context={taskEditModalContext}
                    onSave={saveTasks}
                />
            )}
        </>
    )
}

export { DayCard }
