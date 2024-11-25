import { useCallback } from 'react'
import { TaskModel } from '../models/TaskModel'

interface Output {
    toggleTaskCompleted: (task: TaskModel) => void
    deleteTask: (task: TaskModel) => void
}

interface Props {
    saveTasks: (tasks: TaskModel[]) => void
}

export function useChangeHandlers({ saveTasks }: Props): Output {
    const toggleTaskCompleted = useCallback(
        (task: TaskModel) => {
            const updatedTask = { ...task, completed: !task.completed }
            saveTasks([updatedTask])
        },
        [saveTasks],
    )

    const deleteTask = useCallback(
        (task: TaskModel) => {
            const updatedTask = { ...task, deleted: true }
            saveTasks([updatedTask])
        },
        [saveTasks],
    )

    return { toggleTaskCompleted, deleteTask }
}
