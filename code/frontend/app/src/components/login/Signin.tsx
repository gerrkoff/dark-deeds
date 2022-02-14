import { SigninResultEnum } from 'models'
import * as React from 'react'
import { Form, Message } from 'semantic-ui-react'

import 'styles/login.css'

interface IProps {
    processing: boolean
    signinResult: SigninResultEnum
    signin: (username: string, password: string) => void
    switchForm: (formSignin: boolean) => void
}
interface IState {
    username: string
    password: string
}
export class Signin extends React.PureComponent<IProps, IState> {
    constructor(props: IProps) {
        super(props)
        this.state = { username: '', password: '' }
    }

    public render() {
        const showErrorCredMsg =
            !this.props.processing &&
            this.props.signinResult === SigninResultEnum.WrongUsernamePassword
        return (
            <React.Fragment>
                <Form.Input
                    data-test-id="username-input"
                    label="Username"
                    placeholder="Username"
                    value={this.state.username}
                    onChange={(e: any) =>
                        this.handleInput('username', e.target.value)
                    }
                />
                <Form.Input
                    data-test-id="password-input"
                    label="Password"
                    placeholder="Password"
                    type="password"
                    value={this.state.password}
                    onChange={(e: any) =>
                        this.handleInput('password', e.target.value)
                    }
                />
                <Message
                    negative
                    content="The username or password you entered is incorrect"
                    hidden={!showErrorCredMsg}
                />
                <Form.Button
                    data-test-id="signin-button"
                    onClick={this.handleSubmit}
                    loading={this.props.processing}
                    className="submit-btn"
                    primary
                >
                    Sign in
                </Form.Button>
                <span>
                    Haven't got an account yet?&nbsp;
                    <a href="" onClick={this.handleSwitchForm}>
                        Sign up here.
                    </a>
                </span>
            </React.Fragment>
        )
    }

    private handleInput = (field: string, value: string) => {
        switch (field) {
            case 'username':
                this.setState({ username: value })
                break
            case 'password':
                this.setState({ password: value })
                break
        }
    }

    private handleSubmit = () => {
        this.props.signin(this.state.username, this.state.password)
    }

    private handleSwitchForm = (e: React.MouseEvent<HTMLAnchorElement>) => {
        e.preventDefault()
        this.props.switchForm(false)
    }
}
