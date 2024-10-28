import { useCallback } from 'react'
import { useAppDispatch } from '../../hooks'
import { switchToTab } from '../../app/redux/app-slice'
import { storageService } from '../../common/services/StorageService'
import { taskHubApi } from '../../tasks/api/TaskHubApi'

interface Output {
    signOut: () => void
}

export function useSignOut(): Output {
    const dispatch = useAppDispatch()

    const signOut = useCallback(async (): Promise<void> => {
        await taskHubApi.stop()
        storageService.clearAccessToken()
        dispatch(switchToTab('login'))
    }, [dispatch])

    return { signOut }
}
