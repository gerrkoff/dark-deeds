import { di, diToken } from '../../di'
const dateServiceMock: any = {}
di.rebind(diToken.DateService).toConstantValue(dateServiceMock)

import { recurrencesView as recurrencesViewReducer } from '../../redux/reducers/recurrencesView'
import { IRecurrencesViewState } from '../../redux/types'
import { PlannedRecurrence } from '../../models'
import * as actions from '../../redux/constants'

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

function createRecurrence(id: number): PlannedRecurrence {
    return new PlannedRecurrence(id, '', new Date(), null, null, null, null, false)
}

function setup() {
    dateServiceMock.today = jest.fn().mockImplementation(() => new Date(2010, 10, 10))
}

test('[RECURRENCESVIEW_ADD_RECURRENCE] should return new objects for changed items', () => {
    setup()
    const recurrence1 = createRecurrence(1)
    const recurrence2 = createRecurrence(2)
    const recurrence3 = createRecurrence(3)
    const recurrences = [recurrence1, recurrence2, recurrence3]
    const state = createState(recurrences)
    const action: actions.IAddRecurrence = {
        type: actions.RECURRENCESVIEW_ADD_RECURRENCE
    }
    const result = recurrencesViewReducer(state, action)

    expect(result.plannedRecurrences).not.toBe(recurrences)
    expect(result.plannedRecurrences[0]).toBe(recurrence1)
    expect(result.plannedRecurrences[1]).toBe(recurrence2)
    expect(result.plannedRecurrences[2]).toBe(recurrence3)
})

test('[RECURRENCESVIEW_CHANGE_RECURRENCE] should create new recurrence with default data', () => {
    setup()
    const state = createState([createRecurrence(1)])
    const action: actions.IAddRecurrence = {
        type: actions.RECURRENCESVIEW_ADD_RECURRENCE
    }
    const result = recurrencesViewReducer(state, action)

    expect(result.plannedRecurrences.length).toBe(2)
    expect(result.plannedRecurrences[1].task).toBe('')
    expect(result.plannedRecurrences[1].isDeleted).toBe(false)
    expect(result.plannedRecurrences[1].startDate.getTime()).toBe(new Date(2010, 10, 10).getTime())
})

test('[RECURRENCESVIEW_CHANGE_RECURRENCE] should create recurrence with id = -1 if it\'s first', () => {
    setup()
    const state = createState([createRecurrence(1)])
    const action: actions.IAddRecurrence = {
        type: actions.RECURRENCESVIEW_ADD_RECURRENCE
    }
    const result = recurrencesViewReducer(state, action)

    expect(result.plannedRecurrences[1].id).toBe(-1)
})

test('[RECURRENCESVIEW_CHANGE_RECURRENCE] should create recurrence with the lowest id', () => {
    setup()
    const state = createState([createRecurrence(1), createRecurrence(-100500), createRecurrence(0), createRecurrence(-10)])
    const action: actions.IAddRecurrence = {
        type: actions.RECURRENCESVIEW_ADD_RECURRENCE
    }
    const result = recurrencesViewReducer(state, action)

    expect(result.plannedRecurrences[4].id).toBe(-100501)
})
