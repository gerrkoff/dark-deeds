import { useMemo } from 'react'
import { shallowEqual } from 'react-redux'
import { useAppSelector } from '../../hooks'
import { TaskModel } from '../../tasks/models/TaskModel'
import { TaskEditModalContent } from '../models/TaskEditModalContext'

interface Output {
    conflictTask: TaskModel | null
}

export function useTaskConflictDetection(content: TaskEditModalContent): Output {
    const currentTask = useAppSelector(state => {
        if (content.type === 'EDIT') {
            return state.overview.tasks.find(t => t.uid === content.task.uid) ?? null
        }
        return null
    }, shallowEqual)

    return useMemo(() => {
        const hasConflict =
            content.type === 'EDIT' && currentTask !== null && currentTask.version > content.task.version

        return {
            conflictTask: hasConflict ? currentTask : null,
        }
    }, [content, currentTask])
}
