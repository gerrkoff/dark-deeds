import { ConnectedRouter, connectRouter, routerMiddleware } from 'connected-react-router'
import { createBrowserHistory } from 'history'
import * as React from 'react'
import * as ReactDOM from 'react-dom'
import { Provider } from 'react-redux'
import { applyMiddleware, compose, createStore } from 'redux'
import thunk from 'redux-thunk'

import { AppRouting } from './components/app'
import rootReducer from './redux/reducers'
import { di, diToken, TaskHubApi } from './di'
// import registerServiceWorker from './registerServiceWorker'

import 'react-dragula/dist/dragula.min.css'
import 'react-toastify/dist/ReactToastify.css'
import 'semantic-ui-css/semantic.min.css'
import './index.css'
import './styles/toast.css'

di.get<TaskHubApi>(diToken.TaskHubApi).init()

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
            <AppRouting />
        </ConnectedRouter>
    </Provider>,
    document.getElementById('root') as HTMLElement
)

// TODO: refactor components-containers
// TODO: research
// registerServiceWorker()
