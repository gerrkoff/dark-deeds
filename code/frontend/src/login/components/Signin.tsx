import React from 'react'

interface Props {
    switchToSignup: () => void
    username: string
    setUsername: (username: string) => void
    password: string
    setPassword: (password: string) => void
    signin: () => void
    isLogInPending: boolean
    logInError: string | null
}

function Signin({
    switchToSignup,
    username,
    setUsername,
    password,
    setPassword,
    signin,
    isLogInPending,
    logInError,
}: Props) {
    const handleSwitchToSignup = (e: React.MouseEvent) => {
        e.preventDefault()
        switchToSignup()
    }

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault()
        signin()
    }

    const isSubmitEnabled = username && password && !isLogInPending

    return (
        <form className="p-3" onSubmit={handleSubmit} data-test-id="form-signin">
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
            {logInError && (
                <div className="alert alert-danger" role="alert">
                    {logInError}
                </div>
            )}
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
                    'Sign in'
                )}
            </button>
            <div>
                Haven't got an account yet?{' '}
                <a href="#" onClick={handleSwitchToSignup}>
                    Sign up here.
                </a>
            </div>
        </form>
    )
}

export { Signin }
