import { ConnectedRouter, connectRouter, routerMiddleware } from 'connected-react-router'
import { createBrowserHistory } from 'history'
import * as React from 'react'
import * as ReactDOM from 'react-dom'
import { Provider } from 'react-redux'
import { applyMiddleware, compose, createStore } from 'redux'
import Test from './containers/Test'
import rootReducer from './redux/reducers'
import registerServiceWorker from './registerServiceWorker'

import 'react-dragula/dist/dragula.min.css'
import 'semantic-ui-css/semantic.min.css'
import './index.css'

const history = createBrowserHistory()
const store = createStore<any, any, any, any>(
    connectRouter(history)(rootReducer),
    {},
    compose(
        applyMiddleware(
          routerMiddleware(history)
        )
    )
)

ReactDOM.render(
    <Provider store={store}>
        <ConnectedRouter history={history}>
            <Test />
        </ConnectedRouter>
    </Provider>,
    document.getElementById('root') as HTMLElement
)
registerServiceWorker()
