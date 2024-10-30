import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { App } from './app/App.tsx'
import { store } from './store'
import { Provider } from 'react-redux'
import { DndProvider } from 'react-dnd'
import { HTML5Backend } from 'react-dnd-html5-backend'
import { TouchBackend } from 'react-dnd-touch-backend'

const root = document.getElementById('root')

const isMobile = () => {
    return 'ontouchstart' in window || navigator.maxTouchPoints > 0
}

const dndBackend = isMobile() ? TouchBackend : HTML5Backend
const options = isMobile() ? { delay: 500, ignoreContextMenu: true } : {}

if (root) {
    createRoot(root).render(
        <StrictMode>
            <Provider store={store}>
                <DndProvider backend={dndBackend} options={options}>
                    <App />
                </DndProvider>
            </Provider>
        </StrictMode>,
    )
}
