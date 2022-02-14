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
        hasNotSavedChanges: false,
    }
}

function createRecurrence(
    uid: string,
    isLocal: boolean = false
): PlannedRecurrence {
    return new PlannedRecurrence(
        uid,
        '',
        new Date(),
        null,
        null,
        null,
        null,
        false,
        isLocal
    )
}

test('[RECURRENCESVIEW_DELETE_RECURRENCE] [id greater than 0] should return new objects for changed items', () => {
    const recurrence1 = createRecurrence('1')
    const recurrence2 = createRecurrence('2')
    const recurrence3 = createRecurrence('3')
    const recurrences = [recurrence1, recurrence2, recurrence3]
    const state = createState(recurrences)
    const action: actions.IDeleteRecurrence = {
        type: actions.RECURRENCESVIEW_DELETE_RECURRENCE,
        uid: '2',
    }
    const result = recurrencesViewReducer(state, action)

    expect(result.plannedRecurrences).not.toBe(recurrences)
    expect(result.plannedRecurrences[0]).toBe(recurrence1)
    expect(result.plannedRecurrences[2]).toBe(recurrence3)
    expect(result.plannedRecurrences[1]).not.toBe(recurrence2)
})

test('[RECURRENCESVIEW_DELETE_RECURRENCE] [id greater than 0] should update isDeleted', () => {
    const state = createState([
        createRecurrence('1'),
        createRecurrence('2'),
        createRecurrence('3'),
    ])
    const action: actions.IDeleteRecurrence = {
        type: actions.RECURRENCESVIEW_DELETE_RECURRENCE,
        uid: '2',
    }
    const result = recurrencesViewReducer(state, action)

    expect(
        result.plannedRecurrences.find((x) => x.uid === '2')!.isDeleted
    ).toBe(true)
})

test('[RECURRENCESVIEW_DELETE_RECURRENCE] [id less than 0] should return new objects for changed items', () => {
    const recurrence1 = createRecurrence('1')
    const recurrence2 = createRecurrence('-2', true)
    const recurrence3 = createRecurrence('3')
    const recurrences = [recurrence1, recurrence2, recurrence3]
    const state = createState(recurrences)
    const action: actions.IDeleteRecurrence = {
        type: actions.RECURRENCESVIEW_DELETE_RECURRENCE,
        uid: '-2',
    }
    const result = recurrencesViewReducer(state, action)

    expect(result.plannedRecurrences).not.toBe(recurrences)
    expect(result.plannedRecurrences[0]).toBe(recurrence1)
    expect(result.plannedRecurrences[1]).toBe(recurrence3)
})

test('[RECURRENCESVIEW_DELETE_RECURRENCE] [id less than 0] should delete item', () => {
    const state = createState([
        createRecurrence('1'),
        createRecurrence('-2', true),
        createRecurrence('3'),
    ])
    const action: actions.IDeleteRecurrence = {
        type: actions.RECURRENCESVIEW_DELETE_RECURRENCE,
        uid: '-2',
    }
    const result = recurrencesViewReducer(state, action)

    expect(result.plannedRecurrences.length).toBe(2)
})
