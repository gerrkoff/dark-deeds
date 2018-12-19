import * as React from 'react'
import { Button, Form } from 'semantic-ui-react'
// import { SigninResultEnum } from '../../models'

interface IProps {
    processing: boolean
    // signinResult: SigninResultEnum
    signup: (username: string, password: string) => void
}
interface IState {
    username: string
    password: string
    passwordConfirm: string
}
export class Signup extends React.PureComponent<IProps, IState> {
    constructor(props: IProps) {
        super(props)
        this.state = { username: '', password: '', passwordConfirm: '' }
    }

    public render() {
        // const showErrorCredMsg = !this.props.processing && this.props.signinResult === SigninResultEnum.WrongUsernamePassword
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
                <Form.Field>
                    <label>Confirm password</label>
                    <input placeholder='Confirm password' type='password' value={this.state.passwordConfirm} onChange={(e) => this.handleInput('passwordConfirm', e.target.value)} />
                </Form.Field>
                {/* <Message negative
                    header='Wrong credentials'
                    content='The username or password you entered is incorrect'
                    hidden={!showErrorCredMsg} /> */}
                <Button onClick={this.handleSubmit} loading={this.props.processing}>Submit</Button>
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
            case 'passwordConfirm':
                this.setState({ passwordConfirm: value })
                break
        }
    }

    private handleSubmit = () => {
        this.props.signup(this.state.username, this.state.password)
    }
}
