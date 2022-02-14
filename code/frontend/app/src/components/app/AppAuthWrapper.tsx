import * as React from 'react'
import { ToastContainer } from 'react-toastify'
import { Container, Dimmer, Loader } from 'semantic-ui-react'
import { appearanceService } from 'di/services/appearance-service'
import Login from 'containers/Login'
import { App, IAppProps } from 'components/app'

interface IProps extends IAppProps {
    initialLogginIn: boolean
    userAuthenticated: boolean
    initialLogin: () => void
    loadGeneralInfo: () => void
}
export class AppAuthWrapper extends React.PureComponent<IProps> {
    private appearanceService = appearanceService

    public componentDidMount() {
        this.initialAppLoad()
    }

    public render() {
        return (
            <React.Fragment>
                {this.renderContent()}
                <Dimmer active={this.props.initialLogginIn}>
                    <Loader />
                </Dimmer>
                <ToastContainer />
            </React.Fragment>
        )
    }

    private renderContent = () => {
        if (this.props.initialLogginIn) {
            return <React.Fragment />
        }

        if (this.props.userAuthenticated) {
            return <App {...this.props} />
        } else {
            return (
                <Container>
                    <Login />
                </Container>
            )
        }
    }

    private initialAppLoad = () => {
        this.appearanceService.initTheme()
        this.props.initialLogin()
        this.props.loadGeneralInfo()
    }
}
