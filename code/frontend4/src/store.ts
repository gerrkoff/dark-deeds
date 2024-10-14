import { configureStore } from '@reduxjs/toolkit'
import overviewReducer from './redux/overview/overview-slice'
import logger from 'redux-logger'

export const store = configureStore({
    reducer: {
        overview: overviewReducer,
    },
    middleware: getDefaultMiddleware => getDefaultMiddleware().concat([logger]),
})

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>
// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = typeof store.dispatch
