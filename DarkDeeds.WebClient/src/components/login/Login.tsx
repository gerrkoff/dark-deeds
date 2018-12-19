import * as React from 'react'
import { Form, Segment } from 'semantic-ui-react'
import { SigninResultEnum } from '../../models'
import { Signin } from './Signin'

interface IProps {
    processing: boolean
    signinResult: SigninResultEnum
    signin: (username: string, password: string) => void
}
export class Login extends React.PureComponent<IProps> {
    public render() {
        return (
            <Segment inverted>
                <Form inverted>
                    <Signin processing={this.props.processing} signinResult={this.props.signinResult} signin={this.props.signin} />
                </Form>
            </Segment>
        )
    }
}
