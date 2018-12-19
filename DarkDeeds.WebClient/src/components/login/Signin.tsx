import * as React from 'react'
import { Button, Form, Message } from 'semantic-ui-react'
import { SigninResultEnum } from '../../models'

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
        const showErrorCredMsg = !this.props.processing && this.props.signinResult === SigninResultEnum.WrongUsernamePassword
        return (
            <React.Fragment>
                <Form.Field>
                    <label>Username</label>
                    <input placeholder='Username' value={this.state.username} onChange={(e) => this.handleInput('username', e.target.value)} />
                </Form.Field>
                <Form.Field>
                    <label>Password</label>
                    <input placeholder='Password' type='password' value={this.state.password} onChange={(e) => this.handleInput('password', e.target.value)} />
                </Form.Field>
                <Message negative
                    header='Wrong credentials'
                    content='The username or password you entered is incorrect'
                    hidden={!showErrorCredMsg} />
                <Button onClick={this.handleSubmit} loading={this.props.processing}>Sign in</Button>
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
}
