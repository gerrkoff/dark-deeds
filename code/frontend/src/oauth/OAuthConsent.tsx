import { useCallback, useState } from 'react'
import { unwrapResult } from '@reduxjs/toolkit'
import { Card } from '../common/components/Card'
import { Signin } from '../login/components/Signin'
import { AuthorizePrompt } from './components/AuthorizePrompt'
import { OAuthAuthorizeRequest } from './models/OAuthAuthorizeRequest'
import { oauthApi } from './api/OAuthApi'
import { authService } from '../login/services/AuthService'
import { storageService } from '../common/services/StorageService'
import { signin } from '../login/redux/login-thunk'
import { SigninResultEnum } from '../login/models/SigninResultDto'
import { useAppDispatch, useAppSelector } from '../hooks'

type Phase = 'login' | 'authorize' | 'submitting'

interface Props {
    request: OAuthAuthorizeRequest
}

function OAuthConsent({ request }: Props) {
    const dispatch = useAppDispatch()

    const [phase, setPhase] = useState<Phase>(() => (authService.getCurrentUser() ? 'authorize' : 'login'))
    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const [error, setError] = useState<string | null>(null)

    const { isLogInPending, logInError } = useAppSelector(state => state.login)

    const handleSignin = useCallback(
        async (submittedUsername: string, submittedPassword: string) => {
            const actionResult = await dispatch(signin({ username: submittedUsername, password: submittedPassword }))
            const loginResult = unwrapResult(actionResult)

            if (loginResult.result === SigninResultEnum.Success) {
                storageService.saveAccessToken(loginResult.token)
                setError(null)
                setPhase('authorize')
            }
        },
        [dispatch],
    )

    const handleAuthorize = useCallback(
        async (action: 'allow' | 'deny') => {
            setError(null)
            setPhase('submitting')

            const result = await oauthApi.authorize(action, request)

            if (result.status === 'redirect') {
                window.location.assign(result.redirectUrl)
                return
            }

            if (result.status === 'needs-login') {
                setPhase('login')
                return
            }

            setError('Something went wrong, please try again.')
            setPhase('authorize')
        },
        [request],
    )

    if (phase === 'login') {
        return (
            <Card className="container" style={{ maxWidth: '500px' }}>
                <Signin
                    hideSignup
                    username={username}
                    setUsername={setUsername}
                    password={password}
                    setPassword={setPassword}
                    signin={handleSignin}
                    isLogInPending={isLogInPending}
                    logInError={logInError}
                />
            </Card>
        )
    }

    const currentUser = authService.getCurrentUser()

    return (
        <AuthorizePrompt
            clientId={request.clientId}
            username={currentUser?.username ?? ''}
            error={error}
            isSubmitting={phase === 'submitting'}
            onAuthorize={() => handleAuthorize('allow')}
            onDeny={() => handleAuthorize('deny')}
        />
    )
}

export { OAuthConsent }
