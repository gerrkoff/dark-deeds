import * as React from 'react'
import { ToastContainer } from 'react-toastify'
import { Container, Dimmer, Loader } from 'semantic-ui-react'
import Login from '../../containers/Login'
import { App, IAppProps } from './'

interface IProps extends IAppProps {
    initialLogginIn: boolean
    userAuthenticated: boolean
    initialLogin: () => void
}
export class AppAuthWrapper extends React.PureComponent<IProps> {
    public componentDidMount() {
        this.props.initialLogin()
    }

    public render() {
        return (
            <React.Fragment>
                {this.renderContent()}
                {/* TODO: maybe some other loader */}
                <Dimmer active={this.props.initialLogginIn}>
                    <Loader />
                </Dimmer>
            </React.Fragment>
        )
    }

    private renderContent = () => {
        if (this.props.initialLogginIn) {
            return (<React.Fragment />)
        }

        if (this.props.userAuthenticated) {
            return (<App {...this.props} />)
        } else {
            return (
                // TODO: create common app component
                <Container>
                    <Login />
                    <ToastContainer />
                </Container>
            )
        }
    }
}
