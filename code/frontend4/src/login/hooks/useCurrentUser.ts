import { useCallback } from 'react'
import { useAppDispatch } from '../../hooks'
import { switchToTab } from '../../app/redux/app-slice'
import { fetchCurrentUser } from '../redux/login-thunk'
import { unwrapResult } from '@reduxjs/toolkit'
import { loadSharedSettings } from '../../settings/redux/settings-thunk'
import { loadOverviewTasks } from '../../overview/redux/overview-thunk'
import { taskHubApi } from '../../tasks/api/TaskHubApi'
import {
    taskHubConnected,
    taskHubConnecting,
} from '../../status-panel/redux/status-panel-slice'
import { logout } from '../redux/login-slice'

interface Output {
    loadCurrentUser: () => Promise<void>
    unloadCurrentUser: () => Promise<void>
}

export function useCurrentUser(): Output {
    const dispatch = useAppDispatch()

    const loadCurrentUser = useCallback(async () => {
        try {
            const fetchCurrentUserResult = await dispatch(fetchCurrentUser())

            const currentUserInfo = unwrapResult(fetchCurrentUserResult)

            if (currentUserInfo.userAuthenticated) {
                dispatch(switchToTab('overview'))
                dispatch(loadSharedSettings())
                dispatch(loadOverviewTasks())
                dispatch(taskHubConnecting())
                await taskHubApi.start()
                dispatch(taskHubConnected())
            } else {
                dispatch(switchToTab('login'))
            }
        } catch {
            dispatch(switchToTab('login'))
        }
    }, [dispatch])

    const unloadCurrentUser = useCallback(async () => {
        await taskHubApi.stop()
        dispatch(logout())
        dispatch(switchToTab('login'))
    }, [dispatch])

    return { loadCurrentUser, unloadCurrentUser }
}
