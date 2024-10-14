import { useCallback } from 'react'
import { useAppDispatch } from '../../hooks'
import { switchToTab } from '../../app/redux/app-slice'
import { storageService } from '../../common/services/StorageService'

interface Output {
    signOut: () => void
}

export function useSignOut(): Output {
    const dispatch = useAppDispatch()

    const signOut = useCallback(() => {
        storageService.clearAccessToken()
        dispatch(switchToTab('login'))
    }, [dispatch])

    return { signOut }
}
