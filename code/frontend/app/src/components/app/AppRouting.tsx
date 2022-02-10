import * as React from 'react'
import { BrowserRouter, Route, Switch } from 'react-router-dom'
import AppAuthWrapper from '../../containers/AppAuthWrapper'
import Overview from '../../containers/Overview'
import Settings from '../../containers/Settings'
import RecurrencesView from '../../containers/RecurrencesView'
import { Day } from '../day'
import { NotFound } from './'

export class AppRouting extends React.PureComponent {
    public render() {
        return (
            <AppAuthWrapper>
                <BrowserRouter>
                    <Switch>
                        <Route path='/' exact={true} component={Overview} />
                        <Route path='/day/:date' component={Day} />
                        <Route path='/settings' component={Settings} />
                        <Route path='/recurrences' component={RecurrencesView} />
                        <Route path='*' component={NotFound} />
                    </Switch>
                </BrowserRouter>
            </AppAuthWrapper>
        )
    }
}
