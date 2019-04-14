import * as React from 'react'
import { Form, Message, Popup } from 'semantic-ui-react'
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
    passwordConfirmIncorrect: boolean
}
export class Signup extends React.PureComponent<IProps, IState> {
    constructor(props: IProps) {
        super(props)
        this.state = { username: '', password: '', passwordConfirm: '', passwordConfirmIncorrect: false }
    }

    public render() {
        const inputErrorText = this.inputErrorText()
        const showErrorCredMsg = !this.props.processing && inputErrorText !== ''
        return (
            <React.Fragment>
                <Popup
                    trigger={<Form.Input
                        label='Username'
                        placeholder='Username'
                        value={this.state.username}
                        onChange={(e: any) => this.handleInput('username', e.target.value)}
                    />}
                    content='Your username must contain only digits and letters'
                    on='focus'
                    position='bottom right' />
                <Popup
                    trigger={<Form.Input
                        label='Password'
                        placeholder='Password'
                        type='password'
                        value={this.state.password}
                        onChange={(e: any) => this.handleInput('password', e.target.value)}
                    />}
                    content='Your password must have more than 6 characters and contain at least one lowercase letter, one uppercase letter, one numeric digit, and one special character'
                    on='focus'
                    position='bottom right' />
                <Form.Input
                    label='Confirm password'
                    placeholder='Confirm password'
                    type='password'
                    value={this.state.passwordConfirm}
                    error={this.state.passwordConfirmIncorrect}
                    onChange={(e: any) => this.handleInput('passwordConfirm', e.target.value)} />
                <Message negative
                    content={inputErrorText}
                    hidden={!showErrorCredMsg} />
                <Form.Button
                    onClick={this.handleSubmit}
                    loading={this.props.processing}
                    className='submit-btn'
                    disabled={this.state.passwordConfirmIncorrect}
                    primary>
                    Sign up
                </Form.Button>
                <span>Already have an account?&nbsp;<a href='' onClick={this.handleSwitchForm}>Sign in here.</a></span>
            </React.Fragment>
        )
    }

    private handleInput = (field: string, value: string) => {
        let passwordConfirmIncorrect
        switch (field) {
            case 'username':
                this.setState({ username: value })
                break
            case 'password':
                passwordConfirmIncorrect = this.state.passwordConfirm !== value
                this.setState({ password: value, passwordConfirmIncorrect })
                break
            case 'passwordConfirm':
                passwordConfirmIncorrect = this.state.password !== value
                this.setState({ passwordConfirm: value, passwordConfirmIncorrect })
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
            case SignupResultEnum.InvalidUsername:
                return 'Username is invalid'
            case SignupResultEnum.PasswordInsecure:
                return 'Password is insecure'
            default:
                return ''
        }
    }
}
