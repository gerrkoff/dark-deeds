import * as React from 'react'
import { Form, Segment } from 'semantic-ui-react'
import { SigninResultEnum } from '../../models'
import { Signin, Signup } from './'

interface IProps {
    processing: boolean
    signinResult: SigninResultEnum
    signin: (username: string, password: string) => void
    formSignin: boolean
}
export class Login extends React.PureComponent<IProps> {
    public render() {
        return (
            <Segment inverted>
                <Form inverted>
                    {this.props.formSignin
                        ? <Signin processing={this.props.processing} signinResult={this.props.signinResult} signin={this.props.signin} />
                        : <Signup processing={this.props.processing} signup={(user, pass) => console.log('Signup', user, pass)} />
                    }
                </Form>
            </Segment>
        )
    }
}
