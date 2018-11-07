import { ConnectedRouter, connectRouter, routerMiddleware } from 'connected-react-router'
import { createBrowserHistory } from 'history'
import * as React from 'react'
import * as ReactDOM from 'react-dom'
import { Provider } from 'react-redux'
import { Route, Switch } from 'react-router-dom'
import { applyMiddleware, compose, createStore } from 'redux'
import thunk from 'redux-thunk'
import { NotFound } from './components/app'
import { Day } from './components/day'
import { Settings } from './components/settings'
import App from './containers/App'
import Overview from './containers/Overview'
import rootReducer from './redux/reducers'
// import registerServiceWorker from './registerServiceWorker'

import 'react-dragula/dist/dragula.min.css'
import 'react-toastify/dist/ReactToastify.css'
import 'semantic-ui-css/semantic.min.css'
import './index.css'
import './styles/toast.css'

const history = createBrowserHistory()
const store = createStore<any, any, any, any>(
    connectRouter(history)(rootReducer),
    {},
    compose(
        applyMiddleware(
          routerMiddleware(history),
          thunk
        )
    )
)

ReactDOM.render(
    <Provider store={store}>
        <ConnectedRouter history={history}>
            <App>
                <Switch>
                    <Route path='/' exact={true} component={Overview} />
                    <Route path='/day/:date' component={Day} />
                    <Route path='/settings' component={Settings} />
                    <Route path='*' component={NotFound} />
                </Switch>
            </App>
        </ConnectedRouter>
    </Provider>,
    document.getElementById('root') as HTMLElement
)

// TODO: research
// registerServiceWorker()
