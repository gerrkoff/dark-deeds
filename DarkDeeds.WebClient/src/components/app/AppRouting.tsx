import * as React from 'react'
import { Route, Switch } from 'react-router-dom'
import App from '../../containers/App'
import Overview from '../../containers/Overview'
import { Day } from '../day'
import { Login } from '../login'
import { Settings } from '../settings'
import { NotFound } from './'

interface IProps {
    userAuthenticated: boolean
}
export class AppRouting extends React.PureComponent<IProps> {
    public render() {
        return (
            <React.Fragment>
            { this.props.userAuthenticated
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
                <Switch>
                    <Route path='*' component={Login} />
                </Switch>
            }
            </React.Fragment>
        )
    }
}
