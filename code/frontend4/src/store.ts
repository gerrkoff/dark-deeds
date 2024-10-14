import { configureStore } from '@reduxjs/toolkit'
import overviewReducer from './overview/redux/overview-slice'
import appReducer from './app/redux/app-slice'
import loginReducer from './login/redux/login-slice'
import logger from 'redux-logger'

export const store = configureStore({
    reducer: {
        app: appReducer,
        login: loginReducer,
        overview: overviewReducer,
    },
    middleware: getDefaultMiddleware => getDefaultMiddleware().concat([logger]),
})

export type RootState = ReturnType<typeof store.getState>
export type AppDispatch = typeof store.dispatch
