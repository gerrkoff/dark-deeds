import { recurrencesView as recurrencesViewReducer } from '../../redux/reducers/recurrencesView'
import { IRecurrencesViewState } from '../../redux/types'
import { PlannedRecurrence } from '../../models'
import * as actions from '../../redux/constants'

jest.mock('src/di/services/date-service', () => {
    const mock = {
        dateService: {
            today: jest.fn().mockImplementation(() => {
                return new Date(2010, 10, 10)
            })
        },
    }

    return mock
})

function createState(
    plannedRecurrences: PlannedRecurrence[] = []
    ): IRecurrencesViewState {
    return {
        isCreatingRecurrences: false,
        isLoadingRecurrences: false,
        isSavingRecurrences: false,
        plannedRecurrences,
        edittingRecurrenceId: null,
        hasNotSavedChanges: false
    }
}

function createRecurrence(uid: string): PlannedRecurrence {
    return new PlannedRecurrence(uid, '', new Date(), null, null, null, null, false)
}

// TODO: skip for now
// test('[RECURRENCESVIEW_ADD_RECURRENCE] should return new objects for changed items', () => {
//     const recurrence1 = createRecurrence('1')
//     const recurrence2 = createRecurrence('2')
//     const recurrence3 = createRecurrence('3')
//     const recurrences = [recurrence1, recurrence2, recurrence3]
//     const state = createState(recurrences)
//     const action: actions.IAddRecurrence = {
//         type: actions.RECURRENCESVIEW_ADD_RECURRENCE
//     }
//     const result = recurrencesViewReducer(state, action)

//     expect(result.plannedRecurrences).not.toBe(recurrences)
//     expect(result.plannedRecurrences[0]).toBe(recurrence1)
//     expect(result.plannedRecurrences[1]).toBe(recurrence2)
//     expect(result.plannedRecurrences[2]).toBe(recurrence3)
// })

test('[RECURRENCESVIEW_CHANGE_RECURRENCE] should create new recurrence with default data', () => {
    // setup()
    const state = createState([createRecurrence('1')])
    const action: actions.IAddRecurrence = {
        type: actions.RECURRENCESVIEW_ADD_RECURRENCE
    }
    const result = recurrencesViewReducer(state, action)

    expect(result.plannedRecurrences.length).toBe(2)
    expect(result.plannedRecurrences[1].task).toBe('')
    expect(result.plannedRecurrences[1].isDeleted).toBe(false)
    expect(result.plannedRecurrences[1].startDate.getTime()).toBe(new Date(2010, 10, 10).getTime())
})

test('[RECURRENCESVIEW_CHANGE_RECURRENCE] should create recurrence with some id', () => {
    // setup()
    const state = createState([createRecurrence('1')])
    const action: actions.IAddRecurrence = {
        type: actions.RECURRENCESVIEW_ADD_RECURRENCE
    }
    const result = recurrencesViewReducer(state, action)

    expect(result.plannedRecurrences[1].uid).not.toBeNull()
})
