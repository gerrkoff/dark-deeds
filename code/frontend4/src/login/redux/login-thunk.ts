import { createAsyncThunk } from '@reduxjs/toolkit'
import { loginApi } from '../api/LoginApi'
import { CurrentUserDto } from '../models/CurrentUserDto'

export const fetchCurrentUser = createAsyncThunk(
    'login/fetchCurrentUser',
    async (): Promise<CurrentUserDto> => {
        return await loginApi.fetchCurrentUser()
    },
)
