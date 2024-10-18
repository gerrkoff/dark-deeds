import { createSlice } from '@reduxjs/toolkit'
// import type { PayloadAction } from '@reduxjs/toolkit'
// import { addWithDelay } from './overview-thunk'
import { TaskEntity } from '../../common/models/TaskEntity'
import { dateService } from '../../common/services/DateService'
import { TaskTypeEnum } from '../../common/models/enums/task-type-enum'

// const rows: DayCardModel[][] = []

// for (let row = 0; row < 2; row++) {
//     const dayCards: DayCardModel[] = []

//     for (let i = 1; i < 8; i++) {
//         dayCards.push({
//             date: new Date(2021, row, i),
//             tasks: [
//                 {
//                     uid: `${row}-${i}-0`,
//                     title: 'Task 1',
//                     date: new Date(),
//                     order: 1,
//                     changed: false,
//                     completed: false,
//                     deleted: false,
//                     type: TaskTypeEnum.Additional,
//                     isProbable: false,
//                     version: 1,
//                     time: 1,
//                 },
//                 {
//                     uid: `${row}-${i}-1`,
//                     title: 'Task 2',
//                     date: new Date(),
//                     order: 1,
//                     changed: false,
//                     completed: false,
//                     deleted: false,
//                     type: TaskTypeEnum.Simple,
//                     isProbable: false,
//                     version: 1,
//                     time: 1,
//                 },
//                 {
//                     uid: `${row}-${i}-2`,
//                     title: 'Task 3',
//                     date: new Date(),
//                     order: 2,
//                     changed: false,
//                     completed: true,
//                     deleted: false,
//                     type: TaskTypeEnum.Simple,
//                     isProbable: false,
//                     version: 1,
//                     time: 1,
//                 },
//                 {
//                     uid: `${row}-${i}-3`,
//                     title: 'Task 4',
//                     date: new Date(),
//                     order: 2,
//                     changed: false,
//                     completed: false,
//                     deleted: false,
//                     type: TaskTypeEnum.Simple,
//                     isProbable: true,
//                     version: 1,
//                     time: 1,
//                 },
//                 {
//                     uid: `${row}-${i}-4`,
//                     title: 'Task 5',
//                     date: new Date(),
//                     order: 2,
//                     changed: false,
//                     completed: false,
//                     deleted: false,
//                     type: TaskTypeEnum.Routine,
//                     isProbable: false,
//                     version: 1,
//                     time: 1,
//                 },
//             ],
//         })
//     }

//     rows.push(dayCards)
// }

const initTasks: TaskEntity[] = []
const monday: Date = dateService.monday(dateService.today())
const start: Date = new Date(monday)
start.setDate(monday.getDate() - 3)
const daysAfterMonday: Date = new Date(monday)
daysAfterMonday.setDate(monday.getDate() + 20)

while (start < daysAfterMonday) {
    start.setDate(start.getDate() + 1)

    initTasks.push({
        uid: start.toISOString(),
        changed: false,
        completed: false,
        date: start,
        deleted: false,
        isProbable: false,
        order: 0,
        time: null,
        title: `test ${start.toTimeString()}`,
        type: TaskTypeEnum.Simple,
        version: 1,
    })
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
