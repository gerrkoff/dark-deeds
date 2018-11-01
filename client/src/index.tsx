import * as React from 'react'
import * as ReactDOM from 'react-dom'
import './index.css'
import registerServiceWorker from './registerServiceWorker'

ReactDOM.render(
    <div>Hello World!</div>,
    document.getElementById('root') as HTMLElement
)
registerServiceWorker()
