import { createAsyncThunk } from '@reduxjs/toolkit'
import { TelegramStartDto } from '../models/TelegramStartDto'
import { telegramIntegrationApi } from '../api/TelegramIntegrationApi'
import { dateService } from '../../common/services/DateService'
import { SettingsSharedDto } from '../models/SettingsSharedDto'
import { settingsApi } from '../api/SettingsApi'

export const startTelegram = createAsyncThunk(
    'settings/startTelegram',
    async (): Promise<TelegramStartDto> => {
        return await telegramIntegrationApi.start(
            dateService.getTimezoneOffset(),
        )
    },
)

export const saveSharedSettings = createAsyncThunk(
    'settings/saveSettings',
    async (settings: SettingsSharedDto): Promise<void> => {
        await settingsApi.save(settings)
    },
)

export const loadSharedSettings = createAsyncThunk(
    'settings/loadSettings',
    async (): Promise<SettingsSharedDto> => {
        return await settingsApi.load()
    },
)
