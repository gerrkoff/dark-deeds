import * as React from 'react'
import { Route, Switch } from 'react-router-dom'
import App from '../../containers/App'
import Overview from '../../containers/Overview'
import { Day } from '../day'
import { Settings } from '../settings'
import { NotFound } from './'

export class AppRouting extends React.PureComponent {
    public render() {
        return (
            <App>
                <Switch>
                    <Route path='/' exact={true} component={Overview} />
                    <Route path='/day/:date' component={Day} />
                    <Route path='/settings' component={Settings} />
                    <Route path='*' component={NotFound} />
                </Switch>
            </App>
        )
    }
}
