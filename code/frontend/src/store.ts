import { Action, configureStore, ThunkAction } from '@reduxjs/toolkit'
import overviewReducer from './overview/redux/overview-slice'
import appReducer from './app/redux/app-slice'
import loginReducer from './login/redux/login-slice'
import settingsReducer from './settings/redux/settings-slice'
import statusPanelReducer from './status-panel/redux/status-panel-slice'
import recurrencesRecuder from './recurrences/redux/recurrences-slice'
import toastsReducer from './toasts/redux/toasts-slice'
// import logger from 'redux-logger'

export const store = configureStore({
    reducer: {
        app: appReducer,
        login: loginReducer,
        overview: overviewReducer,
        settings: settingsReducer,
        statusPanel: statusPanelReducer,
        recurrences: recurrencesRecuder,
        toasts: toastsReducer,
    },
    middleware: getDefaultMiddleware =>
        getDefaultMiddleware().concat([
            // logger
        ]),
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
export type AppThunk<ReturnType = void> = ThunkAction<
    ReturnType,
    RootState,
    unknown,
    Action
>
