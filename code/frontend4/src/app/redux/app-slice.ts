import { createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'
import { ApplicationTabType } from '../models/ApplicationTabType'

export interface AppState {
    applicationTab: ApplicationTabType
}

const initialState: AppState = {
    applicationTab: 'overview',
}

export const appSlice = createSlice({
    name: 'overview',
    initialState,
    reducers: {
        switchToTab: (state, action: PayloadAction<ApplicationTabType>) => {
            state.applicationTab = action.payload
        },
    },
})

export const { switchToTab } = appSlice.actions

export default appSlice.reducer
