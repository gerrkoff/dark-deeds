import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { ToastModel } from '../models/ToastModel'

export interface ToastsState {
    toasts: ToastModel[]
}

const initialState: ToastsState = {
    toasts: [],
}

export const toastsSlice = createSlice({
    name: 'toasts',
    initialState,
    reducers: {
        addToast: (state, action: PayloadAction<ToastModel>) => {
            state.toasts.push(action.payload)
        },
        removeToast: (state, action: PayloadAction<string>) => {
            state.toasts = state.toasts.filter(x => x.id !== action.payload)
        },
    },
})

export const { addToast, removeToast } = toastsSlice.actions

export default toastsSlice.reducer
