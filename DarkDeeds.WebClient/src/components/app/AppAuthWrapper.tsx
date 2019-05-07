import * as React from 'react'
import { ToastContainer, Flip } from 'react-toastify'
import { Container, Dimmer, Loader } from 'semantic-ui-react'
import Login from '../../containers/Login'
import { App, IAppProps } from './'

interface IProps extends IAppProps {
    initialLogginIn: boolean
    userAuthenticated: boolean
    initialLogin: () => void
    loadGeneralInfo: () => void
}
export class AppAuthWrapper extends React.PureComponent<IProps> {
    public componentDidMount() {
        this.initialAppLoad()
    }

    public render() {
        return (
            <React.Fragment>
                {this.renderContent()}
                {/* TODO: maybe some other loader */}
                <Dimmer active={this.props.initialLogginIn}>
                    <Loader />
                </Dimmer>
                <ToastContainer newestOnTop={true} transition={Flip} />
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
                <Container>
                    <Login />
                </Container>
            )
        }
    }

    private initialAppLoad = () => {
        this.props.initialLogin()
        this.props.loadGeneralInfo()
    }
}
