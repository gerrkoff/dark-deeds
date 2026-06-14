import { useEffect } from 'react'
import { useAppSelector } from '../../hooks'
import { tasksCacheService } from '../services/TasksCacheService'

export function useTasksCacheTracking() {
    const isTasksCacheHydrated = useAppSelector(state => state.overview.isTasksCacheHydrated)
    const tasks = useAppSelector(state => state.overview.tasks)
    const username = useAppSelector(state => state.login.user?.username)

    useEffect(() => {
        if (!isTasksCacheHydrated || username === undefined) {
            return
        }

        tasksCacheService.save(tasks, username)
    }, [tasks, isTasksCacheHydrated, username])
}
