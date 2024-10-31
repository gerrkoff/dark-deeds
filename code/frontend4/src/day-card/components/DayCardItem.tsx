import { dateService } from '../../common/services/DateService'
import { TaskModel } from '../../tasks/models/TaskModel'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'
import { useTaskItemDnd } from '../../tasks/hooks/useTaskItemDnd'
import { useRef } from 'react'
import { useDayCardItemDndTouchHighlight } from '../hooks/useDayCardItemDndTouchHighlight'
import styles from './DayCard.module.css'

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
    const elementRef = useRef<HTMLElement | null>(null)

    const { dragRef, dropRef, isDragging, isDropping } = useTaskItemDnd({
        task,
        onSaveTasks,
    })

    const { isTouchDndReady } = useDayCardItemDndTouchHighlight({ elementRef })

    let spanClass = 'd-block rounded-1'

    spanClass += ` ${textColor(task, isHighlighted, isDragging, isTouchDndReady)}`
    spanClass += ` ${textDecoration(task)}`
    spanClass += ` ${textBackground(isHighlighted, isDragging, isTouchDndReady)}`

    if (task.type === TaskTypeEnum.Additional) {
        spanClass += ' text-end me-1'
    }

    let liClass = 'border-top'

    if (!isDropping) {
        liClass += ` ${styles.item}`
    }

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
            onClick={e => onOpenTaskMenu(e, task)}
        >
            <span
                ref={node => {
                    elementRef.current = node
                    dragRef(node)
                }}
                onContextMenu={e => e.preventDefault()}
                className={spanClass}
            >
                {text(task, isDebug)}
            </span>
        </li>
    )
}

function textColor(
    task: TaskModel,
    isHighlighted: boolean,
    isDragging: boolean,
    isTouchDndReady: boolean,
): string {
    if (isHighlighted || isDragging || isTouchDndReady) {
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

function textBackground(
    isHighlighted: boolean,
    isDragging: boolean,
    isTouchDndReady: boolean,
): string {
    if (isDragging) {
        return 'bg-primary opacity-50'
    }

    if (isTouchDndReady) {
        return 'bg-primary'
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
