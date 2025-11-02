import { useEffect, useMemo, useState } from 'react'
import { shallowEqual } from 'react-redux'
import { useAppSelector } from '../../hooks'
import { TaskModel } from '../../tasks/models/TaskModel'
import { TaskEditModalContent } from '../models/TaskEditModalContext'

interface Output {
    conflictTask: TaskModel | null
}

export function useTaskConflictDetection(content: TaskEditModalContent): Output {
    const [initialVersion, setInitialVersion] = useState<number | null>(null)

    const currentTask = useAppSelector(state => {
        if (content.type === 'EDIT') {
            return state.overview.tasks.find(t => t.uid === content.task.uid) ?? null
        }
        return null
    }, shallowEqual)

    useEffect(() => {
        if (content.type === 'EDIT') {
            setInitialVersion(content.task.version)
        } else {
            setInitialVersion(null)
        }
    }, [content])

    return useMemo(() => {
        const hasConflict =
            content.type === 'EDIT' &&
            initialVersion !== null &&
            currentTask !== null &&
            currentTask.version > initialVersion

        return {
            conflictTask: hasConflict ? currentTask : null,
        }
    }, [content.type, initialVersion, currentTask])
}
