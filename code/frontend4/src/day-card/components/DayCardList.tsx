import { TaskModel } from '../../tasks/models/TaskModel'
import { DayCardItem } from './DayCardItem'

interface Props {
    tasks: TaskModel[]
    openedMenuTaskUid: string | null
    onOpenTaskMenu: (e: React.MouseEvent<HTMLElement>, task: TaskModel) => void
}

function DayCardList({
    tasks,
    openedMenuTaskUid,
    onOpenTaskMenu: onTaskContextMenu,
}: Props) {
    return (
        <ul className="ps-4 mt-1 mb-2">
            {tasks.map(task => (
                <DayCardItem
                    key={task.uid}
                    task={task}
                    isHighlighted={task.uid === openedMenuTaskUid}
                    onOpenTaskMenu={onTaskContextMenu}
                />
            ))}
        </ul>
    )
}

export { DayCardList }
