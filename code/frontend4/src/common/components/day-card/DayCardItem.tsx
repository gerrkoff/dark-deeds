import { TaskModel } from '../../../tasks/models/TaskModel'
import { TaskTypeEnum } from '../../../tasks/models/TaskTypeEnum'

interface Props {
    task: TaskModel
    onContextMenu: (e: React.MouseEvent<HTMLElement>, task: TaskModel) => void
}

function DayCardItem({ task, onContextMenu }: Props) {
    let spanClass = 'd-block'

    if (task.completed) {
        spanClass += ' text-secondary text-decoration-line-through'
    }

    if (task.isProbable) {
        spanClass += ' fst-italic'
    }

    if (task.type === TaskTypeEnum.Routine) {
        spanClass += ' text-secondary-emphasis '
    }

    if (task.type === TaskTypeEnum.Additional) {
        spanClass += ' text-secondary text-end me-1'
    }

    const liClass = task.type === TaskTypeEnum.Additional ? 'd-block' : ''

    return (
        <li className={liClass} onContextMenu={e => onContextMenu(e, task)}>
            <span className={spanClass}>{task.title}</span>
        </li>
    )
}

export { DayCardItem }
