import { useCallback } from 'react'
import { storageService } from '../../common/services/StorageService'
import { useCurrentUser } from './useCurrentUser'

interface Output {
    signOut: () => void
}

export function useSignOut(): Output {
    const { unloadCurrentUser } = useCurrentUser()

    const signOut = useCallback(async (): Promise<void> => {
        await unloadCurrentUser()
        storageService.clearAccessToken()
    }, [unloadCurrentUser])

    return { signOut }
}
