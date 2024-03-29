import { NotFound } from 'components/app'
import { Day } from 'components/day'
import AppAuthWrapper from 'containers/AppAuthWrapper'
import Overview from 'containers/Overview'
import RecurrencesView from 'containers/RecurrencesView'
import Settings from 'containers/Settings'
import * as React from 'react'
import { Route, Switch } from 'react-router-dom'

export class AppRouting extends React.PureComponent {
    public render() {
        return (
            <AppAuthWrapper>
                <Switch>
                    <Route path="/" exact={true} component={Overview} />
                    <Route path="/day/:date" component={Day} />
                    <Route path="/settings" component={Settings} />
                    <Route path="/recurrences" component={RecurrencesView} />
                    <Route path="*" component={NotFound} />
                </Switch>
            </AppAuthWrapper>
        )
    }
}
