import { TaskModel } from '../../tasks/models/TaskModel'
import { DayCardItem } from './DayCardItem'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'
import clsx from 'clsx'
import styles from './DayCard.module.css'
import { useDayCardDnd } from '../hooks/useDayCardDnd'
import { useDayCardDndList } from '../hooks/useDayCardDndList'
import { useMemo } from 'react'
import { dropZoneBottomId } from '../models/DayCardDndContext'

interface Props {
    date: Date | null
    tasks: TaskModel[]
    openedMenuTaskUid: string | null
    isDebug: boolean
    isRoutineShown: boolean
    onOpenTaskMenu: (e: React.MouseEvent<HTMLElement>, task: TaskModel) => void
    onSaveTasks: (tasks: TaskModel[]) => void
}

function DayCardList({
    date,
    tasks,
    openedMenuTaskUid,
    isDebug,
    isRoutineShown,
    onOpenTaskMenu,
    onSaveTasks,
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
        date,
        tasks: shownTasks,
        onSaveTasks,
    })

    const { listRef, lastItemRef } = useDayCardDndList({
        handleListDragLeave,
        itemDndContext,
    })

    return (
        <>
            <ul
                ref={listRef}
                className="flex-grow-1 d-flex flex-column ps-4 mt-1 mb-0"
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
                        'd-inline border-top flex-grow-1',
                        dropzoneHighlightedTaskUid !== dropZoneBottomId &&
                            styles.item,
                        dropzoneHighlightedTaskUid === dropZoneBottomId &&
                            'border-primary',
                    )}
                    style={{
                        minHeight: '8px',
                    }}
                ></li>
            </ul>
        </>
    )
}

export { DayCardList }
