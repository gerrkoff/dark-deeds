import { configureStore } from '@reduxjs/toolkit'
import overviewReducer from './features/overview/overview-slice'

export const store = configureStore({
    reducer: {
        overview: overviewReducer,
    },
})

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>
// Inferred type: {posts: PostsState, comments: CommentsState, users: UsersState}
export type AppDispatch = typeof store.dispatch
