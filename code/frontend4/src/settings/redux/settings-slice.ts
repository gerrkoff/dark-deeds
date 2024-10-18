import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import {
    loadSharedSettings,
    saveSharedSettings,
    startTelegram,
} from './settings-thunk'
import { OverviewTabEnum } from '../models/OverviewTabEnum'
import { SettingsLocalDto } from '../models/SettingsLocalDto'

export interface SettingsState {
    isStartTelegramPending: boolean
    startTelegramLink: string | null
    isShowCompletedEnabled: boolean
    isSaveSharedSettingsPending: boolean
    isLoadSharedSettingsPending: boolean
    isLocalSettingsLoaded: boolean
    overviewTabsExpanded: OverviewTabEnum[]
}

const initialState: SettingsState = {
    isStartTelegramPending: false,
    startTelegramLink: null,
    isShowCompletedEnabled: true,
    isSaveSharedSettingsPending: false,
    isLoadSharedSettingsPending: false,
    isLocalSettingsLoaded: false,
    overviewTabsExpanded: [],
}

export const settingsSlice = createSlice({
    name: 'settings',
    initialState,
    reducers: {
        changeShowCompleted: (state, action: PayloadAction<boolean>) => {
            state.isShowCompletedEnabled = action.payload
        },
        loadLocalSettings: (state, action: PayloadAction<SettingsLocalDto>) => {
            state.overviewTabsExpanded = action.payload.overviewTabsExpanded
            state.isLocalSettingsLoaded = true
        },
        toggleOverviewTab: (state, action: PayloadAction<OverviewTabEnum>) => {
            state.overviewTabsExpanded = state.overviewTabsExpanded.includes(
                action.payload,
            )
                ? state.overviewTabsExpanded.filter(x => x !== action.payload)
                : [...state.overviewTabsExpanded, action.payload]
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

export const { changeShowCompleted, loadLocalSettings, toggleOverviewTab } =
    settingsSlice.actions

export default settingsSlice.reducer
