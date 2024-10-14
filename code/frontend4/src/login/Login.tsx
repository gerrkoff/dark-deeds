import { useCallback, useState } from 'react'
import { Signin } from './components/Signin'
import { Signup } from './components/Signup'
import { Card } from '../ui/components/Card'
import { useCurrentUserLoader } from './hooks/useCurrentUserLoader'
import { signin, signup } from './redux/login-thunk'
import { useAppDispatch } from '../hooks'
import { unwrapResult } from '@reduxjs/toolkit'
import { SigninResultEnum } from './models/SigninResultDto'
import { SignupResultEnum } from './models/SignupResultDto'
import { storageService } from '../common/services/StorageService'
import { useLoginState } from './redux/login-selectors'
import { resetLogInError } from './redux/login-slice'

function Login() {
    const dispatch = useAppDispatch()

    const [isSignIn, setIsSignIn] = useState(true)
    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const [passwordConfirmation, setPasswordConfirmation] = useState('')

    const { isLogInPending, logInError } = useLoginState()

    const { loadCurrentUser } = useCurrentUserLoader()

    const handleSwitchToSignin = useCallback(() => {
        setIsSignIn(true)
        dispatch(resetLogInError())
    }, [dispatch])

    const handleSwitchToSignup = useCallback(() => {
        setIsSignIn(false)
        dispatch(resetLogInError())
    }, [dispatch])

    const handleSignin = useCallback(async () => {
        const actionResult = await dispatch(signin({ username, password }))
        const loginResult = unwrapResult(actionResult)

        if (loginResult.result === SigninResultEnum.Success) {
            storageService.saveAccessToken(loginResult.token)
            loadCurrentUser()
        }
    }, [dispatch, loadCurrentUser, password, username])

    const handleSignup = useCallback(async () => {
        const actionResult = await dispatch(signup({ username, password }))
        const loginResult = unwrapResult(actionResult)

        if (loginResult.result === SignupResultEnum.Success) {
            storageService.saveAccessToken(loginResult.token)
            loadCurrentUser()
        }
    }, [dispatch, loadCurrentUser, password, username])

    return (
        <Card className="container mt-2" style={{ maxWidth: '500px' }}>
            {isSignIn ? (
                <Signin
                    switchToSignup={handleSwitchToSignup}
                    username={username}
                    setUsername={setUsername}
                    password={password}
                    setPassword={setPassword}
                    signin={handleSignin}
                    isLogInPending={isLogInPending}
                    logInError={logInError}
                />
            ) : (
                <Signup
                    switchToSignin={handleSwitchToSignin}
                    username={username}
                    setUsername={setUsername}
                    password={password}
                    setPassword={setPassword}
                    passwordConfirmation={passwordConfirmation}
                    setPasswordConfirmation={setPasswordConfirmation}
                    signup={handleSignup}
                    isLogInPending={isLogInPending}
                    logInError={logInError}
                />
            )}
        </Card>
    )
}

export { Login }
