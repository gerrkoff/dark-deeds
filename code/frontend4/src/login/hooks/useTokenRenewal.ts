import { useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '../../hooks'
import { loginApi } from '../api/LoginApi'
import { storageService } from '../../common/services/StorageService'
import { authService } from '../services/AuthService'
import { setUser } from '../redux/login-slice'

const oneHourMs = 10_000 // 3600_000
const oneDayMs = 86400_000

const msToTimeString = (ms: number): string => {
    const seconds = Math.floor(ms / 1000)
    const minutes = Math.floor(seconds / 60)
    const hours = Math.floor(minutes / 60)
    const days = Math.floor(hours / 24)

    return `${days}d ${hours % 24}h ${minutes % 60}m ${seconds % 60}s`
}

export function useTokenRenewal() {
    const dispatch = useAppDispatch()

    const { user } = useAppSelector(state => state.login)

    useEffect(() => {
        if (!user) {
            return
        }

        let timeout: NodeJS.Timeout | null = null

        const checkAndRenewTokenIfNeeded = async () => {
            const timeToExpire = user.expiresAt - Date.now()

            console.log(
                `[${new Date().toISOString()}] Token expires in ${msToTimeString(timeToExpire)}`,
            )

            if (timeToExpire < oneDayMs) {
                const renewedToken = await loginApi.renewToken()
                storageService.saveAccessToken(renewedToken)
                const user = authService.getCurrentUser()
                dispatch(setUser(user))
                console.log(`[${new Date().toISOString()}] Token renewed`)
            }

            timeout = setTimeout(checkAndRenewTokenIfNeeded, oneHourMs)
        }

        checkAndRenewTokenIfNeeded()

        return () => {
            if (timeout) {
                clearTimeout(timeout)
            }
        }
    }, [dispatch, user])
}
