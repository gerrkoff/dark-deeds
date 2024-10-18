import { createSlice } from '@reduxjs/toolkit'
import { startTelegram } from './settings-thunk'

export interface SettingsState {
    isStartTelegramPending: boolean
    startTelegramLink: string | null
}

const initialState: SettingsState = {
    isStartTelegramPending: false,
    startTelegramLink: null,
}

export const settingsSlice = createSlice({
    name: 'login',
    initialState,
    reducers: {
        // resetLogInError: state => {
        //     state.logInError = null
        // },
    },
    extraReducers: builder => {
        builder.addCase(startTelegram.pending, state => {
            state.isStartTelegramPending = true
            state.startTelegramLink = null
        })
        builder.addCase(startTelegram.rejected, state => {
            state.isStartTelegramPending = false
            state.startTelegramLink = null
        })
        builder.addCase(startTelegram.fulfilled, (state, action) => {
            state.isStartTelegramPending = false
            state.startTelegramLink = action.payload.url
        })
    },
})

// export const { } = settingsSlice.actions

export default settingsSlice.reducer
