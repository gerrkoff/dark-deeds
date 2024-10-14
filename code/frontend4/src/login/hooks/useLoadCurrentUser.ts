import { useCallback } from 'react'
import { useAppDispatch } from '../../hooks'
import { switchToTab } from '../../app/redux/app-slice'
import { fetchCurrentUser } from '../redux/login-thunk'
import { unwrapResult } from '@reduxjs/toolkit'

interface Output {
    loadCurrentUser: () => Promise<void>
}

export function useLoadCurrentUser(): Output {
    const dispatch = useAppDispatch()

    const loadCurrentUser = useCallback(async () => {
        try {
            const fetchCurrentUserResult = await dispatch(fetchCurrentUser())
            const currentUserInfo = unwrapResult(fetchCurrentUserResult)
            if (currentUserInfo.userAuthenticated) {
                dispatch(switchToTab('overview'))
            } else {
                dispatch(switchToTab('login'))
            }
        } catch {
            dispatch(switchToTab('login'))
        }
    }, [dispatch])

    return { loadCurrentUser }
}
