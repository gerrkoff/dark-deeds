import { createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'

export interface StatusPanelState {
    isSaveTaskPending: boolean
}

const initialState: StatusPanelState = {
    isSaveTaskPending: false,
}

export const statusPanelSlice = createSlice({
    name: 'statusPanel',
    initialState,
    reducers: {
        toggleSaveTaskPending: (state, action: PayloadAction<boolean>) => {
            state.isSaveTaskPending = action.payload
        },
    },
})

export const { toggleSaveTaskPending } = statusPanelSlice.actions

export default statusPanelSlice.reducer
