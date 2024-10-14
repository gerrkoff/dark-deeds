import { createAsyncThunk } from '@reduxjs/toolkit'
import { generalApi } from '../api/GeneralApi'
import { BuildInfoDto } from '../models/BuildInfoDto'

export const fetchBuildInfo = createAsyncThunk(
    'login/fetchBuildInfo',
    async (): Promise<BuildInfoDto> => {
        return await generalApi.fetchBuildInfo()
    },
)
