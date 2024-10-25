import { useRef } from 'react'
import { Card } from '../../common/components/Card'
import { DayCardList } from '../../day-card/components/DayCardList'
import { TaskModel } from '../../tasks/models/TaskModel'
import { useDayCardItemMenu } from '../../day-card/hooks/useDayCardItemMenu'
import { DayCardItemMenu } from '../../day-card/components/DayCardItemMenu'
import { useChangeHandlers } from '../../tasks/hooks/useChangeHandlers'

interface Props {
    tasks: TaskModel[]
    saveTasks: (tasks: TaskModel[]) => void
}

function NoDateSection({ tasks, saveTasks }: Props) {
    const cardRef = useRef<HTMLDivElement>(null)

    const { toggleTaskCompleted, deleteTask } = useChangeHandlers({ saveTasks })

    const { itemMenuContext, openItemMenu, closeItemMenu } = useDayCardItemMenu(
        {
            containerRef: cardRef,
        },
    )

    const editTask = (task: TaskModel) => {
        console.log('Edit task:', task)
    }

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
        </Card>
    )
}

export { NoDateSection }
