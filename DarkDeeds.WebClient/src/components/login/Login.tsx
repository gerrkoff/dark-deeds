import * as React from 'react'
import { Button, Form, Segment } from 'semantic-ui-react'

interface IProps {
    processing: boolean
    signin: (login: string, password: string) => void
}
interface IState {
    login: string
    password: string
}
export class Login extends React.PureComponent<IProps, IState> {
    constructor(props: IProps) {
        super(props)
        this.state = { login: '', password: '' }
    }

    public render() {
        return (
            <Segment inverted>
                <Form inverted>
                    <Form.Field>
                        <label>Login</label>
                        <input placeholder='Login' value={this.state.login} onChange={(e) => this.handleInput('login', e.target.value)} />
                    </Form.Field>
                    <Form.Field>
                        <label>Password</label>
                        <input placeholder='Password' value={this.state.password} onChange={(e) => this.handleInput('password', e.target.value)} />
                    </Form.Field>
                    <Button onClick={this.handleSignin} loading={this.props.processing}>Submit</Button>
                </Form>
            </Segment>
        )
    }

    private handleInput = (field: string, value: string) => {
        switch (field) {
            case 'login':
                this.setState({ login: value })
                break
            case 'password':
                this.setState({ password: value })
                break
        }
    }

    private handleSignin = () => {
        this.props.signin(this.state.login, this.state.password)
    }
}
