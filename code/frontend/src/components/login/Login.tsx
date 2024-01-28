import 'styles/login.css'

import { Signin, Signup } from 'components/login'
import { SigninResultEnum, SignupResultEnum } from 'models'
import * as React from 'react'
import { Form, Segment } from 'semantic-ui-react'

interface IProps {
    processing: boolean
    signinResult: SigninResultEnum
    signin: (username: string, password: string) => void
    formSignin: boolean
    signupResult: SignupResultEnum
    signup: (username: string, password: string) => void
    switchForm: (formSignin: boolean) => void
}
export class Login extends React.PureComponent<IProps> {
    public render() {
        return (
            <Segment
                data-test-id="login-component"
                id="login-container"
                inverted
                raised
            >
                <Form>
                    {this.props.formSignin ? (
                        <Signin
                            processing={this.props.processing}
                            signinResult={this.props.signinResult}
                            signin={this.props.signin}
                            switchForm={this.props.switchForm}
                        />
                    ) : (
                        <Signup
                            processing={this.props.processing}
                            signupResult={this.props.signupResult}
                            signup={this.props.signup}
                            switchForm={this.props.switchForm}
                        />
                    )}
                </Form>
            </Segment>
        )
    }
}
