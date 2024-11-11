import { useCallback, useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '../../hooks'
import { switchToTab } from '../../app/redux/app-slice'
import { fetchCurrentUser, refetchCurrentUser } from '../redux/login-thunk'
import { unwrapResult } from '@reduxjs/toolkit'
import { loadSharedSettings } from '../../settings/redux/settings-thunk'
import { taskHubApi } from '../../tasks/api/TaskHubApi'
import {
    taskHubConnected,
    taskHubConnecting,
} from '../../status-panel/redux/status-panel-slice'
import { logout } from '../redux/login-slice'
import { useTasksSynchronization } from '../../tasks/hooks/useTasksSynchronization'
import { cleanup } from '../../overview/redux/overview-slice'
import { loginApi } from '../api/LoginApi'
import { storageService } from '../../common/services/StorageService'

interface Output {
    loadCurrentUser: () => Promise<void>
    unloadCurrentUser: () => Promise<void>
}

export function useCurrentUser(): Output {
    const dispatch = useAppDispatch()

    const { reloadTasks } = useTasksSynchronization()

    const loadCurrentUser = useCallback(async () => {
        try {
            const fetchCurrentUserResult = await dispatch(fetchCurrentUser())

            const currentUserInfo = unwrapResult(fetchCurrentUserResult)

            if (currentUserInfo.userAuthenticated) {
                dispatch(switchToTab('overview'))
                dispatch(loadSharedSettings())
                reloadTasks()
                dispatch(taskHubConnecting())
                await taskHubApi.start()
                dispatch(taskHubConnected())
            } else {
                dispatch(switchToTab('login'))
            }
        } catch (error) {
            console.error(error)
            dispatch(switchToTab('login'))
        }
    }, [dispatch, reloadTasks])

    const unloadCurrentUser = useCallback(async () => {
        await taskHubApi.stop()
        dispatch(logout())
        dispatch(cleanup())
        dispatch(switchToTab('login'))
    }, [dispatch])

    const { user } = useAppSelector(state => state.login)

    useEffect(() => {
        if (!user) {
            return
        }

        let timeout: NodeJS.Timeout | null = null

        const checkAndRenewTokenIfNeeded = async () => {
            if (user.expiresAt - Date.now() < 300000) {
                const renewedToken = await loginApi.renewToken()
                storageService.saveAccessToken(renewedToken)
                dispatch(refetchCurrentUser())
                console.log(`[${new Date().toISOString()}] Token renewed`)
            }

            timeout = setTimeout(checkAndRenewTokenIfNeeded, 60000)
        }

        timeout = setTimeout(checkAndRenewTokenIfNeeded, 60000)

        return () => {
            if (timeout) {
                clearTimeout(timeout)
            }
        }
    }, [dispatch, user])

    return { loadCurrentUser, unloadCurrentUser }
}
