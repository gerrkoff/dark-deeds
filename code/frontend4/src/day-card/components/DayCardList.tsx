import { TaskModel } from '../../tasks/models/TaskModel'
import { DayCardItem } from './DayCardItem'
import { TaskTypeEnum } from '../../tasks/models/TaskTypeEnum'
import { useTaskDayDnd } from '../../tasks/hooks/useTaskDayDnd'
import clsx from 'clsx'
import styles from './DayCard.module.css'

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
    const shownTasks = isRoutineShown
        ? tasks
        : tasks.filter(task => task.type !== TaskTypeEnum.Routine)

    const lastTask = shownTasks[shownTasks.length - 1]

    const { dropRef, isDropping } = useTaskDayDnd({
        date,
        lastOrder: lastTask?.order ?? 0,
        lastUid: lastTask?.uid,
        onSaveTasks,
    })

    return (
        <>
            <ul className="flex-grow-1 d-flex flex-column ps-4 mt-1 mb-0">
                {shownTasks.map(task => (
                    <DayCardItem
                        key={task.uid}
                        task={task}
                        isDebug={isDebug}
                        isHighlighted={task.uid === openedMenuTaskUid}
                        onOpenTaskMenu={onOpenTaskMenu}
                        onSaveTasks={onSaveTasks}
                    />
                ))}
                <li
                    ref={dropRef}
                    className={clsx(
                        'd-inline border-top flex-grow-1',
                        !isDropping && styles.item,
                        isDropping && 'border-primary',
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
