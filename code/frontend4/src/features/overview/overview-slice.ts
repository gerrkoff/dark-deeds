import { createSlice } from '@reduxjs/toolkit'
import type { PayloadAction } from '@reduxjs/toolkit'
import { addWithDelay } from './overview-thunk'

export interface OverviewState {
    value: number
}

const initialState: OverviewState = {
    value: 0,
}

export const overviewSlice = createSlice({
    name: 'overview',
    initialState,
    reducers: {
        increment: state => {
            state.value += 1
        },
        decrement: state => {
            state.value -= 1
        },
        incrementByAmount: (state, action: PayloadAction<number>) => {
            state.value += action.payload
        },
    },
    extraReducers: builder => {
        builder.addCase(addWithDelay.pending, (state, action) => {
            state.value = -1
            console.log('pending', action)
        })
        builder.addCase(addWithDelay.rejected, (state, action) => {
            state.value = -100
            console.log('rejected', action)
        })
        builder.addCase(addWithDelay.fulfilled, (state, action) => {
            state.value = action.payload
        })
    },
})

export const { increment, decrement, incrementByAmount } = overviewSlice.actions

export default overviewSlice.reducer
