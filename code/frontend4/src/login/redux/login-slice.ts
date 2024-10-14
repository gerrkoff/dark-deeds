import { createSlice } from '@reduxjs/toolkit'
// import type { PayloadAction } from '@reduxjs/toolkit'
import { UserModel } from '../models/UserModel'
import { fetchCurrentUser } from './login-thunk'

export interface LoginState {
    isFetchingUserPending: boolean
    user: UserModel | null
}

const initialState: LoginState = {
    isFetchingUserPending: false,
    user: null,
}

export const loginSlice = createSlice({
    name: 'login',
    initialState,
    reducers: {
        // setUser: (state, action: PayloadAction<UserModel>) => {
        //     state.user = action.payload
        // },
    },
    extraReducers: builder => {
        builder.addCase(fetchCurrentUser.pending, state => {
            state.isFetchingUserPending = true
            state.user = null
        })
        builder.addCase(fetchCurrentUser.rejected, state => {
            state.isFetchingUserPending = false
            state.user = null
        })
        builder.addCase(fetchCurrentUser.fulfilled, (state, action) => {
            state.isFetchingUserPending = false
            state.user = action.payload
        })
    },
})

// export const { setUser } = loginSlice.actions

export default loginSlice.reducer
