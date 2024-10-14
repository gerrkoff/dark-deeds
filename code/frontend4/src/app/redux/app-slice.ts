import { createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'
import { ApplicationTab } from '../models/application-tab-type'

export interface AppState {
    applicationTab: ApplicationTab
}

const initialState: AppState = {
    applicationTab: 'overview',
}

export const appSlice = createSlice({
    name: 'overview',
    initialState,
    reducers: {
        switchToTab: (state, action: PayloadAction<ApplicationTab>) => {
            state.applicationTab = action.payload
        },
    },
})

export const { switchToTab } = appSlice.actions

export default appSlice.reducer
