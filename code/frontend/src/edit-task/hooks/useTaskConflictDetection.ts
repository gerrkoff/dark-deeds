import { useEffect, useMemo, useRef } from 'react'
import { shallowEqual } from 'react-redux'
import { useAppSelector } from '../../hooks'
import { TaskModel } from '../../tasks/models/TaskModel'
import { TaskEditModalContent } from '../models/TaskEditModalContext'
import { SaveFinishSubscription, taskSyncService } from '../../tasks/services/TaskSyncService'

interface Output {
    conflictTask: TaskModel | null
}

export function useTaskConflictDetection(content: TaskEditModalContent): Output {
    const initialVersion = content.type === 'EDIT' ? content.task.version : 0
    const baseVersionRef = useRef<number>(initialVersion)

    // Reset base version when content changes (e.g., opening a different task)
    useEffect(() => {
        baseVersionRef.current = initialVersion
    }, [initialVersion])

    // Subscribe to save finish events to update base version when OUR save completes
    useEffect(() => {
        if (content.type !== 'EDIT') {
            return
        }

        const taskUid = content.task.uid

        const onSaveFinish: SaveFinishSubscription = (_notSaved, savedTasks) => {
            const savedTask = savedTasks.find(t => t.uid === taskUid)
            if (savedTask) {
                // Our save completed - update base version to avoid false conflict
                baseVersionRef.current = savedTask.version
            }
        }

        taskSyncService.subscribeSaveFinish(onSaveFinish)
        return () => taskSyncService.unsubscribeSaveFinish(onSaveFinish)
    }, [content])

    const currentTask = useAppSelector(state => {
        if (content.type === 'EDIT') {
            return state.overview.tasks.find(t => t.uid === content.task.uid) ?? null
        }
        return null
    }, shallowEqual)

    return useMemo(() => {
        const hasConflict =
            content.type === 'EDIT' && currentTask !== null && currentTask.version > baseVersionRef.current

        return {
            conflictTask: hasConflict ? currentTask : null,
        }
    }, [content, currentTask])
}
