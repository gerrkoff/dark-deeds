import * as React from 'react'
import * as ReactDOM from 'react-dom'
import registerServiceWorker from './registerServiceWorker'

import 'react-dragula/dist/dragula.min.css'
import 'semantic-ui-css/semantic.min.css'
import './index.css'

ReactDOM.render(
    <div>Hello World!</div>,
    document.getElementById('root') as HTMLElement
)
registerServiceWorker()
