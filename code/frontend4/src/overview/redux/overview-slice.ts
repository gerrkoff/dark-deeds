import { createSlice } from '@reduxjs/toolkit'
// import type { PayloadAction } from '@reduxjs/toolkit'
// import { addWithDelay } from './overview-thunk'
import { TaskEntity } from '../../common/models/TaskEntity'
import { dateService } from '../../common/services/DateService'
import { TaskTypeEnum } from '../../common/models/enums/task-type-enum'

const initTasks: TaskEntity[] = []
const monday: Date = dateService.monday(dateService.today())
const start: Date = new Date(monday)
start.setDate(monday.getDate() - 3)
const daysAfterMonday: Date = new Date(monday)
daysAfterMonday.setDate(monday.getDate() + 20)

while (start < daysAfterMonday) {
    start.setDate(start.getDate() + 1)
    const d = new Date(start)

    const task = {
        uid: d.toISOString(),
        changed: false,
        completed: false,
        date: d,
        deleted: false,
        isProbable: false,
        order: 0,
        time: null,
        title: `test ${d.toTimeString()}`,
        type: TaskTypeEnum.Simple,
        version: 1,
    }

    console.log(start.toISOString(), '-----', start, '-----', task)

    initTasks.push(task)
}

console.log(initTasks)

export interface OverviewState {
    tasks: TaskEntity[]
}

const initialState: OverviewState = {
    tasks: initTasks,
}

export const overviewSlice = createSlice({
    name: 'overview',
    initialState,
    reducers: {
        // increment: state => {
        //     state.value += 1
        // },
        // decrement: state => {
        //     state.value -= 1
        // },
        // incrementByAmount: (state, action: PayloadAction<number>) => {
        //     state.value += action.payload
        // },
    },
    // extraReducers: builder => {
    // builder.addCase(addWithDelay.pending, (state, action) => {
    //     state.value = -1
    //     console.log('pending', action)
    // })
    // builder.addCase(addWithDelay.rejected, (state, action) => {
    //     state.value = -100
    //     console.log('rejected', action)
    // })
    // builder.addCase(addWithDelay.fulfilled, (state, action) => {
    //     state.value = action.payload
    // })
    // },
})

// export const { increment, decrement, incrementByAmount } = overviewSlice.actions

export default overviewSlice.reducer
