import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { App } from './app/App.tsx'
import { store } from './store.ts'
import { Provider } from 'react-redux'
import './DragDropTouch.js'
import { dragDropAutoScroll } from './DragDropAutoScroll'
import { oauthAuthorizeRequestService } from './oauth/services/OAuthAuthorizeRequestService'
import { OAuthConsent } from './oauth/OAuthConsent'

// Initialize auto-scroll for drag and drop on touch devices
// Comment out the line below to disable auto-scroll
dragDropAutoScroll.init()

const root = document.getElementById('root')

const oauthRequest = oauthAuthorizeRequestService.parse(window.location.search)

if (root) {
    createRoot(root).render(
        <StrictMode>
            <Provider store={store}>{oauthRequest ? <OAuthConsent request={oauthRequest} /> : <App />}</Provider>
        </StrictMode>,
    )
}
