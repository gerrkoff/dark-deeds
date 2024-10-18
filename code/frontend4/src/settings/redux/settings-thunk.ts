import { createAsyncThunk } from '@reduxjs/toolkit'
import { TelegramStartDto } from '../models/TelegramStartDto'
import { telegramIntegrationApi } from '../api/TelegramIntegrationApi'
import { dateService } from '../../common/services/DateService'

export const startTelegram = createAsyncThunk(
    'settings/startTelegram',
    async (): Promise<TelegramStartDto> => {
        return await telegramIntegrationApi.start(
            dateService.getTimezoneOffset(),
        )
    },
)
