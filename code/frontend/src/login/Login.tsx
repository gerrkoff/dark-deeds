import { useCallback, useState } from 'react'
import { Signin } from './components/Signin'
import { Signup } from './components/Signup'
import { Card } from '../common/components/Card'
import { signin, signup } from './redux/login-thunk'
import { useAppDispatch, useAppSelector } from '../hooks'
import { unwrapResult } from '@reduxjs/toolkit'
import { SigninResultEnum } from './models/SigninResultDto'
import { SignupResultEnum } from './models/SignupResultDto'
import { resetLogInError } from './redux/login-slice'
import { useSignIn } from './hooks/useSignIn'

function Login() {
    const dispatch = useAppDispatch()

    const [isSignIn, setIsSignIn] = useState(true)
    const [username, setUsername] = useState('')
    const [password, setPassword] = useState('')
    const [passwordConfirmation, setPasswordConfirmation] = useState('')

    const { isLogInPending, logInError } = useAppSelector(state => state.login)

    const { signIn } = useSignIn()

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
            signIn(loginResult.token)
        }
    }, [dispatch, signIn, password, username])

    const handleSignup = useCallback(async () => {
        const actionResult = await dispatch(signup({ username, password }))
        const logInResult = unwrapResult(actionResult)

        if (logInResult.result === SignupResultEnum.Success) {
            signIn(logInResult.token)
        }
    }, [dispatch, signIn, password, username])

    return (
        <Card className="container" style={{ maxWidth: '500px' }}>
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
