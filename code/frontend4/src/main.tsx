// import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { App } from './app/App.tsx'
import { store } from './store'
import { Provider } from 'react-redux'
import { DndProvider } from 'react-dnd'
import { HTML5Backend } from 'react-dnd-html5-backend'

const root = document.getElementById('root')

if (root) {
    createRoot(root).render(
        // <StrictMode>
        <Provider store={store}>
            <DndProvider backend={HTML5Backend}>
                <App />
            </DndProvider>
        </Provider>,
        // </StrictMode>,
    )
}

// dnd
// server push
// fix modal (too many nodes)
// icons
// day cards
// edit task model
// tests
