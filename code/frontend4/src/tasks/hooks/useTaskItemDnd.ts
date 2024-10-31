import { dndType } from '../../common/utils/dnd'
import { TaskModel } from '../models/TaskModel'
import {
    ConnectDragSource,
    ConnectDropTarget,
    useDrag,
    useDrop,
} from 'react-dnd'

interface Output {
    dragRef: ConnectDragSource
    dropRef: ConnectDropTarget
    isDragging: boolean
    isDropping: boolean
}

interface Props {
    task: TaskModel
    onSaveTasks: (tasks: TaskModel[]) => void
}

export function useTaskItemDnd({ task, onSaveTasks }: Props): Output {
    const [{ isDragging }, dragRef] = useDrag(
        () => ({
            type: dndType.task,
            item: task,
            collect: monitor => ({
                isDragging: monitor.isDragging(),
            }),
        }),
        [task],
    )

    const [{ isDropping }, dropRef] = useDrop(
        () => ({
            accept: dndType.task,
            collect: monitor => ({
                isDropping: monitor.isOver() && monitor.canDrop(),
            }),
            canDrop: (draggedTask: TaskModel): boolean =>
                draggedTask.uid !== task.uid,
            drop: (draggedTask: TaskModel) => {
                onSaveTasks([
                    {
                        ...draggedTask,
                        date: task.date,
                        order: task.order - 0.5,
                    },
                ])
            },
        }),
        [task, onSaveTasks],
    )

    return { dragRef, dropRef, isDragging, isDropping }
}
