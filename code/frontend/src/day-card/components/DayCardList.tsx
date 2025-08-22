import { TaskModel } from '../../tasks/models/TaskModel'
import { DayCardItem } from './DayCardItem'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'
import clsx from 'clsx'
import styles from './DayCard.module.css'
import { useDayCardDnd } from '../hooks/useDayCardDnd'
import { useDayCardDndList } from '../hooks/useDayCardDndList'
import { memo, useMemo } from 'react'
import { dropZoneBottomId } from '../models/DayCardDndContext'

interface Props {
    tasks: TaskModel[]
    openedMenuTaskUid: string | null
    isDebug: boolean
    isRoutineShown: boolean
    onOpenTaskMenu: (e: React.MouseEvent<HTMLElement>, task: TaskModel) => void
    onSaveTasks: (tasks: TaskModel[]) => void
    onTransformDrop: (task: TaskModel) => TaskModel
}

function DayCardList({
    tasks,
    openedMenuTaskUid,
    isDebug,
    isRoutineShown,
    onOpenTaskMenu,
    onSaveTasks,
    onTransformDrop,
}: Props) {
    const shownTasks = useMemo(
        () =>
            isRoutineShown
                ? tasks
                : tasks.filter(task => task.type !== TaskTypeEnum.Routine),
        [isRoutineShown, tasks],
    )

    const {
        draggedTaskUid,
        dropzoneHighlightedTaskUid,
        handleListDragLeave,
        itemDndContext,
    } = useDayCardDnd({
        tasks: shownTasks,
        onSaveTasks,
        onTransformDrop,
    })

    const { listRef, lastItemRef } = useDayCardDndList({
        handleListDragLeave,
        itemDndContext,
    })

    return (
        <>
            <ul
                ref={listRef}
                className={clsx(
                    'flex-grow-1 d-flex flex-column ps-4 mt-1 mb-0',
                    styles.list,
                )}
            >
                {shownTasks.map(task => (
                    <DayCardItem
                        key={task.uid}
                        task={task}
                        isDebug={isDebug}
                        isHighlighted={task.uid === openedMenuTaskUid}
                        isDragged={task.uid === draggedTaskUid}
                        isDraggedOver={task.uid === dropzoneHighlightedTaskUid}
                        itemDndContext={itemDndContext}
                        onOpenTaskMenu={onOpenTaskMenu}
                    />
                ))}
                <li
                    ref={lastItemRef}
                    className={clsx(
                        'd-inline flex-grow-1',
                        dropzoneHighlightedTaskUid !== dropZoneBottomId &&
                            styles.item,
                        dropzoneHighlightedTaskUid === dropZoneBottomId &&
                            'border-top border-primary',
                    )}
                    style={{
                        boxSizing: 'initial',
                        minHeight: '8px',
                    }}
                ></li>
            </ul>
        </>
    )
}

const memoized = memo(DayCardList)

export { memoized as DayCardList }
