import { useCallback, useEffect } from 'react'
import { useAppDispatch } from '../../hooks'
import { switchToTab } from '../../app/redux/app-slice'
import { loadSharedSettings } from '../../settings/redux/settings-thunk'
import { taskHubApi } from '../../tasks/api/TaskHubApi'
import { taskHubConnected, taskHubConnecting } from '../../status-panel/redux/status-panel-slice'
import { setUser } from '../redux/login-slice'
import { useTasksSynchronization } from '../../tasks/hooks/useTasksSynchronization'
import { cleanup, hydrateTasks } from '../../overview/redux/overview-slice'
import { authService } from '../services/AuthService'
import { tasksCacheService } from '../../tasks/services/TasksCacheService'
import { addToast } from '../../toasts/redux/toasts-slice'
import { api } from '../../common/api/Api'

interface Output {
    loadCurrentUser: () => Promise<void>
    unloadCurrentUser: () => Promise<void>
}

export function useCurrentUser(): Output {
    const dispatch = useAppDispatch()

    const { reloadTasks } = useTasksSynchronization()

    const resetToLogin = useCallback(() => {
        tasksCacheService.clear()
        dispatch(setUser(null))
        dispatch(cleanup())
        dispatch(switchToTab('login'))
    }, [dispatch])

    // The server rejects requests with 401 when the stored token is invalid or expired - log the
    // user out instead of silently showing stale cached tasks.
    useEffect(() => {
        api.setUnauthorizedHandler(() => {
            void taskHubApi.stop()
            resetToLogin()
            dispatch(
                addToast({
                    text: 'Your session has expired. Please sign in again.',
                    category: 'session-expired',
                }),
            )
        })
    }, [dispatch, resetToLogin])

    const loadCurrentUser = useCallback(async () => {
        try {
            const user = authService.getCurrentUser()
            dispatch(setUser(user))

            if (!user) {
                dispatch(switchToTab('login'))
                return
            }

            dispatch(switchToTab('overview'))
            api.resetUnauthorized()
            dispatch(hydrateTasks(tasksCacheService.load()))
            dispatch(loadSharedSettings())
            reloadTasks()
            dispatch(taskHubConnecting())
            await taskHubApi.start()
            if (taskHubApi.isConnected()) {
                dispatch(taskHubConnected())
            }
        } catch (error) {
            console.error(error)
            dispatch(switchToTab('login'))
        }
    }, [dispatch, reloadTasks])

    const unloadCurrentUser = useCallback(async () => {
        await taskHubApi.stop()
        resetToLogin()
    }, [resetToLogin])

    return { loadCurrentUser, unloadCurrentUser }
}
