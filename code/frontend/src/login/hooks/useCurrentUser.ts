import { useCallback, useEffect } from 'react'
import { useAppDispatch } from '../../hooks'
import { switchToTab } from '../../app/redux/app-slice'
import { loadSharedSettings } from '../../settings/redux/settings-thunk'
import { loadLocalSettings } from '../../settings/redux/settings-slice'
import { localSettingsService } from '../../settings/services/LocalSettingsService'
import { taskHubApi } from '../../tasks/api/TaskHubApi'
import { taskHubConnected, taskHubConnecting } from '../../status-panel/redux/status-panel-slice'
import { setUser } from '../redux/login-slice'
import { useTasksSynchronization } from '../../tasks/hooks/useTasksSynchronization'
import { cleanup, hydrateTasks } from '../../overview/redux/overview-slice'
import { authService } from '../services/AuthService'
import { tasksCacheService } from '../../tasks/services/TasksCacheService'
import { taskSyncService } from '../../tasks/services/TaskSyncService'
import { addToast } from '../../toasts/redux/toasts-slice'
import { api } from '../../common/api/Api'
import { storageService } from '../../common/services/StorageService'

interface Output {
    loadCurrentUser: () => Promise<void>
    unloadCurrentUser: () => Promise<void>
}

export function useCurrentUser(): Output {
    const dispatch = useAppDispatch()

    const { reloadTasks } = useTasksSynchronization()

    const goToLogin = useCallback(() => {
        dispatch(setUser(null))
        dispatch(cleanup())
        dispatch(switchToTab('login'))
    }, [dispatch])

    // The server rejects requests with 401 when the stored token is invalid or expired. Log the
    // user out in memory, but keep the persisted data so the same user replays unsaved edits on
    // re-login (no data lost on involuntary session expiry); a different user has it wiped on login.
    useEffect(() => {
        api.setUnauthorizedHandler(() => {
            void taskHubApi.stop()
            taskSyncService.reset()
            goToLogin()
            dispatch(
                addToast({
                    text: 'Your session has expired. Please sign in again.',
                    category: 'session-expired',
                    autoDismissMs: 6000,
                }),
            )
        })
    }, [dispatch, goToLogin])

    const loadCurrentUser = useCallback(async () => {
        try {
            const user = authService.getCurrentUser()
            dispatch(setUser(user))

            if (!user) {
                dispatch(switchToTab('login'))
                return
            }

            // The locally stored data belongs to one user at a time. If it belongs to someone else
            // (e.g. a different user signs in on the same browser), wipe it so nothing leaks.
            const dataOwner = storageService.loadDataOwner()
            if (dataOwner !== null && dataOwner !== user.username) {
                storageService.clearUserData()
            }
            storageService.saveDataOwner(user.username)

            dispatch(switchToTab('overview'))
            api.resetUnauthorized()
            dispatch(hydrateTasks(tasksCacheService.load()))
            taskSyncService.restoreOutbox()
            dispatch(loadLocalSettings(localSettingsService.load()))
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

    // Explicit sign-out: the user is intentionally leaving, so purge all of their locally stored
    // data, not just the in-memory state.
    const unloadCurrentUser = useCallback(async () => {
        await taskHubApi.stop()
        taskSyncService.reset()
        storageService.clearUserData()
        goToLogin()
    }, [goToLogin])

    return { loadCurrentUser, unloadCurrentUser }
}
