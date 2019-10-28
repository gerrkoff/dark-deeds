import * as React from 'react'
import { Route, Switch } from 'react-router-dom'
import AppAuthWrapper from '../../containers/AppAuthWrapper'
import Overview from '../../containers/Overview'
import Settings from '../../containers/Settings'
import { Day } from '../day'
import { RecurrencesView } from '../recurrence'
import { NotFound } from './'

export class AppRouting extends React.PureComponent {
    public render() {
        return (
            <AppAuthWrapper>
                <Switch>
                    <Route path='/' exact={true} component={Overview} />
                    <Route path='/day/:date' component={Day} />
                    <Route path='/settings' component={Settings} />
                    <Route path='/recurrence' component={RecurrencesView} />
                    <Route path='*' component={NotFound} />
                </Switch>
            </AppAuthWrapper>
        )
    }
}
