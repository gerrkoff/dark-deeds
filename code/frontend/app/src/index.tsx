// import registerServiceWorker from './registerServiceWorker'
import 'react-dragula/dist/dragula.min.css'
import 'react-toastify/dist/ReactToastify.css'
import 'semantic-ui-css/semantic.min.css'
import 'index.css'
import 'styles/toast.css'

import { AppRouting } from 'components/app'
import { ConnectedRouter, routerMiddleware } from 'connected-react-router'
import { taskHubApi } from 'di/api/task-hub-api'
import { createBrowserHistory } from 'history'
import * as React from 'react'
import * as ReactDOM from 'react-dom'
import { Provider } from 'react-redux'
import { applyMiddleware, compose, createStore } from 'redux'
import createRootReducer from 'redux/reducers'
import thunk from 'redux-thunk'

taskHubApi.init()

const history = createBrowserHistory()
const store = createStore<any, any, any, any>(
    createRootReducer(history),
    {},
    compose(applyMiddleware(routerMiddleware(history), thunk))
)

ReactDOM.render(
    <Provider store={store}>
        <ConnectedRouter history={history}>
            <AppRouting />
        </ConnectedRouter>
    </Provider>,
    document.getElementById('root') as HTMLElement
)

// TODO: refactor components-containers
// TODO: research
// registerServiceWorker()
