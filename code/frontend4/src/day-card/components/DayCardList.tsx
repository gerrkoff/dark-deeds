import { TaskModel } from '../../tasks/models/TaskModel'
import { DayCardItem } from './DayCardItem'

interface Props {
    tasks: TaskModel[]
    openedMenuTaskUid: string | null
    isDebug: boolean
    onOpenTaskMenu: (e: React.MouseEvent<HTMLElement>, task: TaskModel) => void
}

function DayCardList({
    tasks,
    openedMenuTaskUid,
    isDebug,
    onOpenTaskMenu: onTaskContextMenu,
}: Props) {
    return (
        <ul className="ps-4 mt-1 mb-2">
            {tasks.map(task => (
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
