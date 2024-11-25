import { createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'

export interface StatusPanelState {
    isSaveTaskPending: boolean
    isTaskHubConnected: boolean
    isTaskHubConnecting: boolean
}

const initialState: StatusPanelState = {
    isSaveTaskPending: false,
    isTaskHubConnected: false,
    isTaskHubConnecting: false,
}

export const statusPanelSlice = createSlice({
    name: 'statusPanel',
    initialState,
    reducers: {
        toggleSaveTaskPending: (state, action: PayloadAction<boolean>) => {
            state.isSaveTaskPending = action.payload
        },
        taskHubConnected: state => {
            state.isTaskHubConnected = true
            state.isTaskHubConnecting = false
        },
        taskHubDisconnected: state => {
            state.isTaskHubConnected = false
            state.isTaskHubConnecting = false
        },
        taskHubConnecting: state => {
            state.isTaskHubConnecting = true
        },
    },
})

export const {
    toggleSaveTaskPending,
    taskHubConnected,
    taskHubDisconnected,
    taskHubConnecting,
} = statusPanelSlice.actions

export default statusPanelSlice.reducer
