// import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { App } from './app/App.tsx'
import { store } from './store'
import { Provider } from 'react-redux'

const root = document.getElementById('root')

if (root) {
    createRoot(root).render(
        // <StrictMode>
        <Provider store={store}>
            <App />
        </Provider>,
        // </StrictMode>,
    )
}

// item menu
// dnd
// server push
// day cards
// edit task model
// tests
