import * as React from 'react'
import { Button, Form, Message, Segment } from 'semantic-ui-react'
import { SigninResultEnum } from '../../models'

interface IProps {
    processing: boolean
    signinResult: SigninResultEnum
    signin: (username: string, password: string) => void
}
interface IState {
    username: string
    password: string
}
export class Login extends React.PureComponent<IProps, IState> {
    constructor(props: IProps) {
        super(props)
        this.state = { username: '', password: '' }
    }

    public render() {
        const showErrorCredMsg = !this.props.processing && this.props.signinResult === SigninResultEnum.WrongUsernamePassword
        return (
            <Segment inverted>
                <Form inverted>
                    <Form.Field>
                        <label>Username</label>
                        <input placeholder='Username' value={this.state.username} onChange={(e) => this.handleInput('username', e.target.value)} />
                    </Form.Field>
                    <Form.Field>
                        <label>Password</label>
                        <input placeholder='Password' value={this.state.password} onChange={(e) => this.handleInput('password', e.target.value)} />
                    </Form.Field>
                    <Message negative
                        header='Wrong credentials'
                        content='The username or password you entered is incorrect'
                        hidden={!showErrorCredMsg} />
                    <Button onClick={this.handleSignin} loading={this.props.processing}>Submit</Button>
                </Form>
            </Segment>
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

    private handleSignin = () => {
        this.props.signin(this.state.username, this.state.password)
    }
}
