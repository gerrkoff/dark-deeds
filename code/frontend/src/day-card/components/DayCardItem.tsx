import { dateService } from '../../common/services/DateService'
import { TaskModel } from '../../tasks/models/TaskModel'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'
import styles from './DayCard.module.css'
import { DayCardItemDndContext } from '../models/DayCardDndContext'
import { useDayCardDndItem } from '../hooks/useDayCardDndItem'
import { memo } from 'react'

interface Props {
    task: TaskModel
    isHighlighted: boolean
    isDragged: boolean
    isDraggedOver: boolean
    isDebug: boolean
    itemDndContext: DayCardItemDndContext
    onOpenTaskMenu: (e: React.MouseEvent<HTMLElement>, task: TaskModel) => void
}

function DayCardItem({
    task,
    isHighlighted,
    isDragged,
    isDraggedOver,
    isDebug,
    itemDndContext,
    onOpenTaskMenu,
}: Props) {
    const { dragRef, dropRef } = useDayCardDndItem({
        task,
        itemDndContext,
    })

    let spanClass = 'd-block rounded-1'

    spanClass += ` ${textColor(task, isHighlighted, isDragged)}`
    spanClass += ` ${textDecoration(task)}`
    spanClass += ` ${textBackground(isHighlighted, isDragged)}`

    if (task.type === TaskTypeEnum.Additional) {
        spanClass += ' text-end me-1'
    }

    let liClass = ''

    if (!isDraggedOver) {
        liClass += ` ${styles.item}`
    }

    if (task.type === TaskTypeEnum.Additional) {
        liClass += ' d-inline'
    }

    if (isDraggedOver) {
        liClass += ' border-top border-primary'
    }

    return (
        <li
            ref={dropRef}
            className={liClass}
            style={{ cursor: 'pointer', padding: '2px 2px' }}
            onClick={e => onOpenTaskMenu(e, task)}
        >
            <span draggable ref={dragRef} className={spanClass}>
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

    if (
        task.type === TaskTypeEnum.Routine ||
        task.type === TaskTypeEnum.Weekly
    ) {
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

const memoized = memo(DayCardItem)

export { memoized as DayCardItem }
