import { createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'
import { ApplicationTabType } from '../models/ApplicationTabType'
import { fetchBuildInfo } from './app-thunk'

export interface AppState {
    applicationTab: ApplicationTabType
    appVersion: string
}

const initialState: AppState = {
    applicationTab: 'login',
    appVersion: '',
}

export const appSlice = createSlice({
    name: 'app',
    initialState,
    reducers: {
        switchToTab: (state, action: PayloadAction<ApplicationTabType>) => {
            state.applicationTab = action.payload
        },
    },
    extraReducers: builder => {
        builder.addCase(fetchBuildInfo.fulfilled, (state, action) => {
            state.appVersion = action.payload.appVersion
            console.log('appVersion:', state.appVersion)
        })
    },
})

export const { switchToTab } = appSlice.actions

export default appSlice.reducer
