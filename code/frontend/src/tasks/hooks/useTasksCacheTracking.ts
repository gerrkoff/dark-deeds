import { useEffect } from 'react'
import { useAppSelector } from '../../hooks'
import { tasksCacheService } from '../services/TasksCacheService'

export function useTasksCacheTracking() {
    const isTasksCacheHydrated = useAppSelector(state => state.overview.isTasksCacheHydrated)
    const tasks = useAppSelector(state => state.overview.tasks)

    useEffect(() => {
        if (!isTasksCacheHydrated) {
            return
        }

        tasksCacheService.save(tasks)
    }, [tasks, isTasksCacheHydrated])
}
