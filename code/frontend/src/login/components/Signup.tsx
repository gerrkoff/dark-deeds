import clsx from 'clsx'
import { useRef } from 'react'
import { isKeyEnter } from '../../common/utils/keys'

interface Props {
    switchToSignin: () => void
    username: string
    setUsername: (username: string) => void
    password: string
    setPassword: (password: string) => void
    passwordConfirmation: string
    setPasswordConfirmation: (passwordConfirmation: string) => void
    signup: (username: string, password: string) => void
    isLogInPending: boolean
    logInError: string | null
}

function Signup({
    switchToSignin,
    username,
    setUsername,
    password,
    setPassword,
    passwordConfirmation,
    setPasswordConfirmation,
    signup,
    isLogInPending,
    logInError,
}: Props) {
    const usernameRef = useRef<HTMLInputElement>(null)
    const passwordRef = useRef<HTMLInputElement>(null)
    const passwordConfirmationRef = useRef<HTMLInputElement>(null)

    const handleSwitchToSignin = (e: React.MouseEvent) => {
        e.preventDefault()
        switchToSignin()
    }

    const isPasswordConfirmationValid = password === passwordConfirmation

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault()
        const submittedUsername = usernameRef.current?.value ?? username
        const submittedPassword = passwordRef.current?.value ?? password
        const submittedConfirmation = passwordConfirmationRef.current?.value ?? passwordConfirmation
        setUsername(submittedUsername)
        setPassword(submittedPassword)
        setPasswordConfirmation(submittedConfirmation)
        if (
            !submittedUsername ||
            !submittedPassword ||
            !submittedConfirmation ||
            submittedPassword !== submittedConfirmation ||
            isLogInPending
        )
            return
        signup(submittedUsername, submittedPassword)
    }

    const isSubmitEnabled = !isLogInPending

    const handleKeyDown = (e: React.KeyboardEvent<HTMLFormElement>) => {
        if (isKeyEnter(e) && !isLogInPending) {
            e.preventDefault()
            e.currentTarget.requestSubmit()
        }
    }

    return (
        <form className="p-3" onSubmit={handleSubmit} onKeyDown={handleKeyDown}>
            <div className="form-floating mb-3">
                <input
                    ref={usernameRef}
                    type="text"
                    className="form-control"
                    id="username"
                    name="username"
                    autoComplete="username"
                    placeholder="Username"
                    value={username}
                    onChange={e => setUsername(e.target.value)}
                />
                <label htmlFor="username">Username</label>
            </div>
            <div className="form-floating mb-3">
                <input
                    ref={passwordRef}
                    type="password"
                    className="form-control"
                    id="password"
                    name="password"
                    autoComplete="new-password"
                    placeholder="Password"
                    value={password}
                    onChange={e => setPassword(e.target.value)}
                />
                <label htmlFor="password">Password</label>
            </div>
            <div className="form-floating mb-3">
                <input
                    ref={passwordConfirmationRef}
                    type="password"
                    className={clsx('form-control', {
                        'is-invalid': !isPasswordConfirmationValid,
                    })}
                    id="passwordConfirmation"
                    name="passwordConfirmation"
                    autoComplete="new-password"
                    placeholder="Confirm password"
                    value={passwordConfirmation}
                    onChange={e => setPasswordConfirmation(e.target.value)}
                />
                <label htmlFor="passwordConfirmation">Confirm password</label>
            </div>
            {logInError && (
                <div className="alert alert-danger" role="alert">
                    {logInError}
                </div>
            )}
            <div className="alert alert-info" role="alert">
                Your password needs to be at least 8 characters long
            </div>
            <button
                type="submit"
                className="btn btn-primary mb-3"
                style={{ minWidth: '120px' }}
                disabled={!isSubmitEnabled}
            >
                {isLogInPending ? (
                    <>
                        <span
                            className="spinner-border spinner-border-sm"
                            aria-hidden="true"
                            data-test-id="btn-loader"
                        ></span>
                    </>
                ) : (
                    'Sign up'
                )}
            </button>
            <div>
                Already have an account?{' '}
                <a href="#" onClick={handleSwitchToSignin}>
                    Sign in here.
                </a>
            </div>
        </form>
    )
}

export { Signup }
