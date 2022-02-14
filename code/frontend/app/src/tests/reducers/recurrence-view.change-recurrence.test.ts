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

function createRecurrence(uid: string, task: string): PlannedRecurrence {
    return new PlannedRecurrence(
        uid,
        task,
        new Date(),
        null,
        null,
        null,
        null,
        false
    )
}

test('[RECURRENCESVIEW_CHANGE_RECURRENCE] should return new objects for changed items', () => {
    const recurrence1 = createRecurrence('1', '')
    const recurrence2 = createRecurrence('2', '')
    const recurrence3 = createRecurrence('3', '')
    const recurrences = [recurrence1, recurrence2, recurrence3]
    const state = createState(recurrences)
    recurrence2.task = 'update'
    const action: actions.IChangeRecurrence = {
        type: actions.RECURRENCESVIEW_CHANGE_RECURRENCE,
        plannedRecurrence: recurrence2,
    }
    const result = recurrencesViewReducer(state, action)

    expect(result.plannedRecurrences).not.toBe(recurrences)
    expect(result.plannedRecurrences[0]).toBe(recurrence1)
    expect(result.plannedRecurrences[2]).toBe(recurrence3)
    expect(result.plannedRecurrences[1]).not.toBe(recurrence2)
})

test('[RECURRENCESVIEW_CHANGE_RECURRENCE] should update task', () => {
    const state = createState([
        createRecurrence('1', ''),
        createRecurrence('2', ''),
        createRecurrence('3', ''),
    ])
    const action: actions.IChangeRecurrence = {
        type: actions.RECURRENCESVIEW_CHANGE_RECURRENCE,
        plannedRecurrence: createRecurrence('2', 'updated'),
    }
    const result = recurrencesViewReducer(state, action)

    expect(result.plannedRecurrences.find((x) => x.uid === '2')!.task).toBe(
        'updated'
    )
})
