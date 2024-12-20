import { createAsyncThunk } from '@reduxjs/toolkit'
import { loginApi } from '../api/LoginApi'
import { SigninResultDto } from '../models/SigninResultDto'
import { SignupResultDto } from '../models/SignupResultDto'

export const signin = createAsyncThunk(
    'login/signin',
    async ({
        username,
        password,
    }: {
        username: string
        password: string
    }): Promise<SigninResultDto> => {
        return await loginApi.signin(username, password)
    },
)

export const signup = createAsyncThunk(
    'login/signup',
    async ({
        username,
        password,
    }: {
        username: string
        password: string
    }): Promise<SignupResultDto> => {
        return await loginApi.signup(username, password)
    },
)
