import { useCallback } from 'react'
import { storageService } from '../../common/services/StorageService'
import { useCurrentUser } from './useCurrentUser'

interface Output {
    signIn: (accessToken: string) => Promise<void>
}

export function useSignIn(): Output {
    const { loadCurrentUser } = useCurrentUser()

    const signIn = useCallback(
        async (accessToken: string) => {
            storageService.saveAccessToken(accessToken)
            loadCurrentUser()
        },
        [loadCurrentUser],
    )

    return { signIn }
}
