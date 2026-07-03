import { useRef } from 'react'
import { isKeyEnter } from '../../common/utils/keys'

interface Props {
    switchToSignup?: () => void
    username: string
    setUsername: (username: string) => void
    password: string
    setPassword: (password: string) => void
    signin: (username: string, password: string) => void
    isLogInPending: boolean
    logInError: string | null
    hideSignup?: boolean
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
    hideSignup = false,
}: Props) {
    const usernameRef = useRef<HTMLInputElement>(null)
    const passwordRef = useRef<HTMLInputElement>(null)

    const handleSwitchToSignup = (e: React.MouseEvent) => {
        e.preventDefault()
        switchToSignup?.()
    }

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault()
        const submittedUsername = usernameRef.current?.value ?? username
        const submittedPassword = passwordRef.current?.value ?? password
        setUsername(submittedUsername)
        setPassword(submittedPassword)
        if (!submittedUsername || !submittedPassword || isLogInPending) return
        signin(submittedUsername, submittedPassword)
    }

    const isSubmitEnabled = !isLogInPending

    const handleKeyDown = (e: React.KeyboardEvent<HTMLFormElement>) => {
        if (isKeyEnter(e) && !isLogInPending) {
            e.preventDefault()
            e.currentTarget.requestSubmit()
        }
    }

    return (
        <form className="p-3" onSubmit={handleSubmit} onKeyDown={handleKeyDown} data-test-id="form-signin">
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
                    autoComplete="current-password"
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
            {!hideSignup && (
                <div>
                    Haven't got an account yet?{' '}
                    <a href="#" onClick={handleSwitchToSignup}>
                        Sign up here.
                    </a>
                </div>
            )}
        </form>
    )
}

export { Signin }
