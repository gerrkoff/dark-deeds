import { useRef, memo, useCallback } from 'react'
import { Card } from '../../common/components/Card'
import { DayCardList } from '../../day-card/components/DayCardList'
import { TaskModel } from '../../tasks/models/TaskModel'
import { DayCardItemMenu } from '../../day-card/components/DayCardItemMenu'
import { EditTaskModal } from '../../edit-task/EditTaskModal'
import { useEditTaskModal } from '../../edit-task/hooks/useEditTaskModal'
import { useDayCardMenuItem } from '../../day-card/hooks/useDayCardMenuItem'
import { taskTransformService } from '../../common/services/TaskTransformService'

interface Props {
    tasks: TaskModel[]
    isDebug: boolean
    saveTasks: (tasks: TaskModel[]) => void
}

function NoDateSection({ tasks, isDebug, saveTasks }: Props) {
    const cardRef = useRef<HTMLDivElement>(null)

    const { taskEditModalContext, openTaskEditModal } = useEditTaskModal()

    const { itemMenuContext, openItemMenu, closeItemMenu, deleteTask, editTask, toggleTaskCompleted } =
        useDayCardMenuItem({
            containerRef: cardRef,
            saveTasks,
            openTaskEditModal,
        })

    const transformDrop = useCallback((task: TaskModel) => taskTransformService.toNoDate(task), [])

    return (
        <Card elementRef={cardRef} style={{ fontSize: '0.8rem', minHeight: '70px' }} dataTestId="card-no-date">
            <DayCardList
                tasks={tasks}
                isDebug={isDebug}
                isRoutineShown={true}
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

            {taskEditModalContext && <EditTaskModal context={taskEditModalContext} onSave={saveTasks} />}
        </Card>
    )
}

const memoized = memo(NoDateSection)

export { memoized as NoDateSection }
