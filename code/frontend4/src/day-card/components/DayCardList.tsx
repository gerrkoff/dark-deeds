import { TaskModel } from '../../tasks/models/TaskModel'
import { DayCardItem } from './DayCardItem'

interface Props {
    tasks: TaskModel[]
    onOpenTaskMenu: (e: React.MouseEvent<HTMLElement>, task: TaskModel) => void
}

function DayCardList({ tasks, onOpenTaskMenu: onTaskContextMenu }: Props) {
    return (
        <ul className="ps-4 mt-1 mb-2">
            {tasks.map(task => (
                <DayCardItem
                    key={task.uid}
                    task={task}
                    onOpenTaskMenu={onTaskContextMenu}
                />
            ))}
        </ul>
    )
}

export { DayCardList }
