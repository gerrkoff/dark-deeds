import * as React from 'react'
import { Form, Message } from 'semantic-ui-react'
import { SignupResultEnum } from '../../models'

import '../../styles/login.css'

interface IProps {
    processing: boolean
    signupResult: SignupResultEnum
    signup: (username: string, password: string) => void
    switchForm: (formSignin: boolean) => void
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
        const inputErrorText = this.inputErrorText()
        const showErrorCredMsg = !this.props.processing && inputErrorText !== ''
        return (
            <React.Fragment>
                <Form.Input
                    label='Username'
                    placeholder='Username'
                    value={this.state.username}
                    onChange={(e: any) => this.handleInput('username', e.target.value)} />
                <Message info
                    header='Enter secure password'
                    // TODO: adjust rule
                    content='Here is password rule' />
                <Form.Input
                    label='Password'
                    placeholder='Password'
                    type='password'
                    value={this.state.password}
                    onChange={(e: any) => this.handleInput('password', e.target.value)} />
                <Form.Input
                    label='Confirm password'
                    placeholder='Confirm password'
                    type='password'
                    value={this.state.passwordConfirm}
                    onChange={(e: any) => this.handleInput('passwordConfirm', e.target.value)} />
                <Message negative
                    header='Incorrect data'
                    content={inputErrorText}
                    hidden={!showErrorCredMsg} />
                <Form.Button onClick={this.handleSubmit} loading={this.props.processing} className='submit-btn'>Sign up</Form.Button>
                <span>Already have an account?&nbsp;<a href='' onClick={this.handleSwitchForm}>Sign in here.</a></span>
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

    private handleSwitchForm = (e: React.MouseEvent<HTMLAnchorElement>) => {
        e.preventDefault()
        this.props.switchForm(true)
    }

    private inputErrorText = (): string => {
        switch (this.props.signupResult) {
            case SignupResultEnum.UsernameAlreadyExists:
                return 'User already exists'
            case SignupResultEnum.PasswordInsecure:
                return 'Password is insecure'
            default:
                return ''
        }
    }
}
