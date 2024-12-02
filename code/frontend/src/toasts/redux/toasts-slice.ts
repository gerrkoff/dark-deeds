import { createSlice, PayloadAction } from '@reduxjs/toolkit'
import { ToastModel } from '../models/ToastModel'
import { AddToastModel } from '../models/AddToastModel'
import { uuidv4 } from '../../common/utils/uuidv4'

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
        addToast: (state, action: PayloadAction<AddToastModel>) => {
            if (action.payload.category) {
                const index = state.toasts.findIndex(
                    x => x.category === action.payload.category,
                )
                if (index !== -1) {
                    state.toasts[index].counter =
                        state.toasts[index].counter + 1
                    return
                }
            }

            state.toasts.push({
                id: uuidv4(),
                type: action.payload.type ?? '',
                text: action.payload.text,
                category: action.payload.category ?? '',
                counter: 0,
            })
        },
        removeToast: (state, action: PayloadAction<string>) => {
            state.toasts = state.toasts.filter(x => x.id !== action.payload)
        },
    },
})

export const { addToast, removeToast } = toastsSlice.actions

export default toastsSlice.reducer
