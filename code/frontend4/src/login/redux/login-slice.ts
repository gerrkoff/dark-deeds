import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { UserModel } from '../models/UserModel'
import { signin, signup } from './login-thunk'
import { SigninResultEnum } from '../models/SigninResultDto'
import { SignupResultEnum } from '../models/SignupResultDto'

export interface LoginState {
    user: UserModel | null
    isLogInPending: boolean
    logInError: string | null
}

const initialState: LoginState = {
    user: null,
    isLogInPending: false,
    logInError: null,
}

export const loginSlice = createSlice({
    name: 'login',
    initialState,
    reducers: {
        setUser: (state, action: PayloadAction<UserModel | null>) => {
            state.user = action.payload
        },
        resetLogInError: state => {
            state.logInError = null
        },
    },
    extraReducers: builder => {
        builder.addCase(signin.pending, state => {
            state.isLogInPending = true
            state.logInError = null
        })
        builder.addCase(signin.rejected, state => {
            state.isLogInPending = false
            state.logInError = 'Unknown error, try again later'
        })
        builder.addCase(signin.fulfilled, (state, action) => {
            state.isLogInPending = false
            state.logInError = null
            if (action.payload.result === SigninResultEnum.Success) {
                state.logInError = null
            } else if (
                action.payload.result === SigninResultEnum.WrongUsernamePassword
            ) {
                state.logInError = 'Wrong username or password'
            } else {
                state.logInError = 'Unknown error, try again later'
            }
        })

        builder.addCase(signup.pending, state => {
            state.isLogInPending = true
            state.logInError = null
        })
        builder.addCase(signup.rejected, state => {
            state.isLogInPending = false
            state.logInError = 'Unknown error, try again later'
        })
        builder.addCase(signup.fulfilled, (state, action) => {
            state.isLogInPending = false
            if (action.payload.result === SignupResultEnum.Success) {
                state.logInError = null
            } else if (
                action.payload.result === SignupResultEnum.InvalidUsername
            ) {
                state.logInError = 'Username is invalid'
            } else if (
                action.payload.result === SignupResultEnum.PasswordInsecure
            ) {
                state.logInError = 'Password is insecure'
            } else if (
                action.payload.result === SignupResultEnum.UsernameAlreadyExists
            ) {
                state.logInError = 'User already exists'
            } else {
                state.logInError = 'Unknown error, try again later'
            }
        })
    },
})

export const { setUser, resetLogInError } = loginSlice.actions

export default loginSlice.reducer
