import * as React from 'react'
import * as ReactDOM from 'react-dom'
import { Provider } from 'react-redux'
import { createStore } from 'redux'
import Test from './containers/Test'
import rootReducer from './redux/reducers'
import registerServiceWorker from './registerServiceWorker'

import 'react-dragula/dist/dragula.min.css'
import 'semantic-ui-css/semantic.min.css'
import './index.css'

const store = createStore<any, any, any, any>(rootReducer)

ReactDOM.render(
    <Provider store={store}>
        <Test />
    </Provider>,
    document.getElementById('root') as HTMLElement
)
registerServiceWorker()
