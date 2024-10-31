import { TaskModel } from '../models/TaskModel'
import { ConnectDropTarget, useDrop } from 'react-dnd'
import { dndType } from '../../common/utils/dnd'

interface Output {
    dropRef: ConnectDropTarget
    isDropping: boolean
}

interface Props {
    date: Date | null
    lastUid: string | null
    lastOrder: number
    onSaveTasks: (tasks: TaskModel[]) => void
}

export function useTaskDayDnd({
    date,
    lastOrder,
    lastUid,
    onSaveTasks,
}: Props): Output {
    const [{ isDropping }, dropRef] = useDrop(
        () => ({
            accept: dndType.task,
            collect: monitor => ({
                isDropping: monitor.isOver() && monitor.canDrop(),
            }),
            canDrop: (draggedTask: TaskModel): boolean =>
                draggedTask.uid !== lastUid,
            drop: (draggedTask: TaskModel) => {
                onSaveTasks([
                    {
                        ...draggedTask,
                        date: date ? date.valueOf() : null,
                        order: lastOrder + 0.5,
                    },
                ])
            },
        }),
        [onSaveTasks, date, lastOrder, lastUid],
    )

    return { dropRef, isDropping }
}
