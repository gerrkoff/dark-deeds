import clsx from 'clsx'
import React from 'react'

interface Props {
    switchToSignin: () => void
    username: string
    setUsername: (username: string) => void
    password: string
    setPassword: (password: string) => void
    passwordConfirmation: string
    setPasswordConfirmation: (passwordConfirmation: string) => void
    signup: () => void
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
    const handleSwitchToSignin = (e: React.MouseEvent) => {
        e.preventDefault()
        switchToSignin()
    }

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault()
        signup()
    }

    const isSubmitEnabled = username && password && passwordConfirmation && !isLogInPending

    const isPasswordConfirmationValid = password === passwordConfirmation

    return (
        <form className="p-3" onSubmit={handleSubmit}>
            <div className="form-floating mb-3">
                <input
                    type="text"
                    className="form-control"
                    id="username"
                    placeholder="Username"
                    value={username}
                    onChange={e => setUsername(e.target.value)}
                />
                <label htmlFor="username">Username</label>
            </div>
            <div className="form-floating mb-3">
                <input
                    type="password"
                    className="form-control"
                    id="password"
                    placeholder="Password"
                    value={password}
                    onChange={e => setPassword(e.target.value)}
                />
                <label htmlFor="password">Password</label>
            </div>
            <div className="form-floating mb-3">
                <input
                    type="password"
                    className={clsx('form-control', {
                        'is-invalid': !isPasswordConfirmationValid,
                    })}
                    id="passwordConfirmation"
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
