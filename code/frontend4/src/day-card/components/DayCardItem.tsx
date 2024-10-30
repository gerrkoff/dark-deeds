import { dateService } from '../../common/services/DateService'
import { TaskModel } from '../../tasks/models/TaskModel'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'

interface Props {
    task: TaskModel
    isHighlighted: boolean
    onOpenTaskMenu: (e: React.MouseEvent<HTMLElement>, task: TaskModel) => void
}

function DayCardItem({ task, isHighlighted, onOpenTaskMenu }: Props) {
    let spanClass = 'd-block'

    spanClass += ` ${textColor(task, isHighlighted)}`
    spanClass += ` ${textDecoration(task)}`

    if (isHighlighted) {
        spanClass += ' rounded text-bg-secondary'
    }

    if (task.type === TaskTypeEnum.Additional) {
        spanClass += ' text-end me-1'
    }

    const liClass = task.type === TaskTypeEnum.Additional ? 'd-block' : ''

    return (
        <li className={liClass} onClick={e => onOpenTaskMenu(e, task)}>
            <span className={spanClass}>{text(task)}</span>
        </li>
    )
}

function textColor(task: TaskModel, isHighlighted: boolean): string {
    if (isHighlighted) {
        return ''
    }

    if (task.completed || task.type === TaskTypeEnum.Additional) {
        return 'text-secondary'
    }

    if (task.type === TaskTypeEnum.Routine) {
        return 'text-secondary-emphasis'
    }

    return ''
}

function textDecoration(task: TaskModel): string {
    if (task.completed) {
        return 'text-decoration-line-through'
    }

    if (task.isProbable) {
        return 'fst-italic'
    }

    return ''
}

function text(task: TaskModel): string {
    let text = `[${task.order}]`
    if (task.time !== null) {
        text += ` ${dateService.toTimeLabel(task.time)}`
    }
    text += ` ${task.title}`
    text += ` v${task.version}`

    return text
}

export { DayCardItem }
