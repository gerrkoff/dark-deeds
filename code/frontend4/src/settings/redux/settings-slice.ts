import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import {
    loadSharedSettings,
    saveSharedSettings,
    startTelegram,
} from './settings-thunk'

export interface SettingsState {
    isStartTelegramPending: boolean
    startTelegramLink: string | null
    isShowCompletedEnabled: boolean
    isSaveSharedSettingsPending: boolean
    isLoadSharedSettingsPending: boolean
}

const initialState: SettingsState = {
    isStartTelegramPending: false,
    startTelegramLink: null,
    isShowCompletedEnabled: true,
    isSaveSharedSettingsPending: false,
    isLoadSharedSettingsPending: false,
}

export const settingsSlice = createSlice({
    name: 'login',
    initialState,
    reducers: {
        changeShowCompleted: (state, action: PayloadAction<boolean>) => {
            state.isShowCompletedEnabled = action.payload
        },
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

        builder.addCase(saveSharedSettings.pending, state => {
            state.isSaveSharedSettingsPending = true
        })
        builder.addCase(saveSharedSettings.rejected, state => {
            state.isSaveSharedSettingsPending = false
        })
        builder.addCase(saveSharedSettings.fulfilled, state => {
            state.isSaveSharedSettingsPending = false
        })

        builder.addCase(loadSharedSettings.pending, state => {
            state.isLoadSharedSettingsPending = true
        })
        builder.addCase(loadSharedSettings.rejected, state => {
            state.isLoadSharedSettingsPending = false
        })
        builder.addCase(loadSharedSettings.fulfilled, (state, action) => {
            state.isLoadSharedSettingsPending = false
            state.isShowCompletedEnabled = action.payload.showCompleted
        })
    },
})

export const { changeShowCompleted } = settingsSlice.actions

export default settingsSlice.reducer
