import * as React from 'react'
import { Route, Switch } from 'react-router-dom'
import { Container, Dimmer, Loader } from 'semantic-ui-react'
import App from '../../containers/App'
import Overview from '../../containers/Overview'
import { Day } from '../day'
import { Login } from '../login'
import { Settings } from '../settings'
import { NotFound } from './'

interface IProps {
    initialLogginIn: boolean
    userAuthenticated: boolean
    initialLogin: () => void
}
export class AppRouting extends React.PureComponent<IProps> {
    public componentDidMount() {
        this.props.initialLogin()
    }

    public render() {
        return (
            <React.Fragment>
                {this.props.userAuthenticated
                    ?
                    <App>
                        <Switch>
                            <Route path='/' exact={true} component={Overview} />
                            <Route path='/day/:date' component={Day} />
                            <Route path='/settings' component={Settings} />
                            <Route path='*' component={NotFound} />
                        </Switch>
                    </App>
                    :
                    <Container>
                        <Switch>
                            <Route path='*' component={Login} />
                        </Switch>
                    </Container>
                }
                {/* TODO: maybe some other loader */}
                <Dimmer active={this.props.initialLogginIn}>
                    <Loader />
                </Dimmer>
            </React.Fragment>
        )
    }
}
