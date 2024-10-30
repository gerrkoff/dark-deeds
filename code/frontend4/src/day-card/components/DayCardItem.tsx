import { dateService } from '../../common/services/DateService'
import { TaskModel } from '../../tasks/models/TaskModel'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'
import { useTaskItemDnd } from '../../tasks/hooks/useTaskItemDnd'

interface Props {
    task: TaskModel
    isHighlighted: boolean
    isDebug: boolean
    onOpenTaskMenu: (e: React.MouseEvent<HTMLElement>, task: TaskModel) => void
    onSaveTasks: (tasks: TaskModel[]) => void
}

function DayCardItem({
    task,
    isHighlighted,
    isDebug,
    onOpenTaskMenu,
    onSaveTasks,
}: Props) {
    const { dragRef, dropRef, isDragging, isDropping } = useTaskItemDnd({
        task,
        onSaveTasks,
    })

    let spanClass = 'd-block rounded-1'

    spanClass += ` ${textColor(task, isHighlighted, isDragging)}`
    spanClass += ` ${textDecoration(task)}`
    spanClass += ` ${textBackground(isHighlighted, isDragging)}`

    if (task.type === TaskTypeEnum.Additional) {
        spanClass += ' text-end me-1'
    }

    let liClass = 'border-top'

    if (task.type === TaskTypeEnum.Additional) {
        liClass += ' d-inline'
    }

    if (isDropping) {
        liClass += ' border-top border-primary'
    }

    return (
        <li
            ref={dropRef}
            className={liClass}
            style={{
                borderColor: !isDropping
                    ? 'var(--bs-dark-bg-subtle) !important'
                    : '',
            }}
            onClick={e => onOpenTaskMenu(e, task)}
        >
            <span ref={dragRef} className={spanClass}>
                {text(task, isDebug)}
            </span>
        </li>
    )
}

function textColor(
    task: TaskModel,
    isHighlighted: boolean,
    isDragging: boolean,
): string {
    if (isHighlighted || isDragging) {
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

function textBackground(isHighlighted: boolean, isDragging: boolean): string {
    if (isDragging) {
        return 'bg-primary opacity-50'
    }

    if (isHighlighted) {
        return 'text-bg-secondary'
    }

    return ''
}

function text(task: TaskModel, isDebug: boolean): string {
    let text = ''
    if (isDebug) {
        text += ` [${task.order}]`
    }
    if (task.time !== null) {
        text += ` ${dateService.toTimeLabel(task.time)}`
    }
    text += ` ${task.title}`
    if (isDebug) {
        text += ` v${task.version}`
    }

    return text.trimStart()
}

export { DayCardItem }
