import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { App } from './app/App.tsx'
import { store } from './store.ts'
import { Provider } from 'react-redux'
import './DragDropTouch.js'

const root = document.getElementById('root')

if (root) {
    createRoot(root).render(
        <StrictMode>
            <Provider store={store}>
                <App />
            </Provider>
        </StrictMode>,
    )
}
