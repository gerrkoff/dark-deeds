import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { App } from './app/App.tsx'
import { store } from './store.ts'
import { Provider } from 'react-redux'
import './DragDropTouch.js'
import { dragDropAutoScroll } from './DragDropAutoScroll'

// Initialize auto-scroll for drag and drop on touch devices
// Comment out the line below to disable auto-scroll
dragDropAutoScroll.init()

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
