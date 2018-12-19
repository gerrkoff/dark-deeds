import * as React from 'react'
import { Form, Segment } from 'semantic-ui-react'
import { SigninResultEnum, SignupResultEnum } from '../../models'
import { Signin, Signup } from './'

interface IProps {
    processing: boolean
    signinResult: SigninResultEnum
    signin: (username: string, password: string) => void
    formSignin: boolean
    signupResult: SignupResultEnum
    signup: (username: string, password: string) => void
}
export class Login extends React.PureComponent<IProps> {
    public render() {
        return (
            <Segment inverted>
                <Form inverted>
                    {this.props.formSignin
                        ? <Signin processing={this.props.processing} signinResult={this.props.signinResult} signin={this.props.signin} />
                        : <Signup processing={this.props.processing} signupResult={this.props.signupResult} signup={this.props.signup} />
                    }
                </Form>
            </Segment>
        )
    }
}
