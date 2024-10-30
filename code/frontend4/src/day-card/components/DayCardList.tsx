import { TaskModel } from '../../tasks/models/TaskModel'
import { DayCardItem } from './DayCardItem'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'

interface Props {
    tasks: TaskModel[]
    openedMenuTaskUid: string | null
    isDebug: boolean
    isRoutineShown: boolean
    onOpenTaskMenu: (e: React.MouseEvent<HTMLElement>, task: TaskModel) => void
}

function DayCardList({
    tasks,
    openedMenuTaskUid,
    isDebug,
    isRoutineShown,
    onOpenTaskMenu: onTaskContextMenu,
}: Props) {
    const shownTasks = isRoutineShown
        ? tasks
        : tasks.filter(task => task.type !== TaskTypeEnum.Routine)

    return (
        <ul className="ps-4 mt-1 mb-2">
            {shownTasks.map(task => (
                <DayCardItem
                    key={task.uid}
                    task={task}
                    isDebug={isDebug}
                    isHighlighted={task.uid === openedMenuTaskUid}
                    onOpenTaskMenu={onTaskContextMenu}
                />
            ))}
        </ul>
    )
}

export { DayCardList }
