import { PlannedRecurrence } from '../../models'

export const RECURRENCESVIEW_CREATING_RECURRENCES_PROCESSING = 'RECURRENCESVIEW_CREATING_RECURRENCES_PROCESSING'
export interface ICreateRecurrencesProcessing {
    type: typeof RECURRENCESVIEW_CREATING_RECURRENCES_PROCESSING
}

export const RECURRENCESVIEW_CREATING_RECURRENCES_FINISH = 'RECURRENCESVIEW_CREATING_RECURRENCES_FINISH'
export interface ICreateRecurrencesFinish {
    type: typeof RECURRENCESVIEW_CREATING_RECURRENCES_FINISH
}

export const RECURRENCESVIEW_LOADING_RECURRENCES_PROCESSING = 'RECURRENCESVIEW_LOADING_RECURRENCES_PROCESSING'
export interface ILoadRecurrencesProcessing {
    type: typeof RECURRENCESVIEW_LOADING_RECURRENCES_PROCESSING
}

export const RECURRENCESVIEW_LOADING_RECURRENCES_FAIL = 'RECURRENCESVIEW_LOADING_RECURRENCES_FAIL'
export interface ILoadRecurrencesFail {
    type: typeof RECURRENCESVIEW_LOADING_RECURRENCES_FAIL
}

export const RECURRENCESVIEW_LOADING_RECURRENCES_SUCCESS = 'RECURRENCESVIEW_LOADING_RECURRENCES_SUCCESS'
export interface ILoadRecurrencesSuccess {
    type: typeof RECURRENCESVIEW_LOADING_RECURRENCES_SUCCESS
    plannedRecurrences: PlannedRecurrence[]
}

export const RECURRENCESVIEW_CHANGE_EDITTING_RECURRENCE = 'RECURRENCESVIEW_CHANGE_EDITTING_RECURRENCE'
export interface IChangeEdittingRecurrence {
    type: typeof RECURRENCESVIEW_CHANGE_EDITTING_RECURRENCE
    edittingRecurrenceId: string | null
}

export const RECURRENCESVIEW_CHANGE_RECURRENCE = 'RECURRENCESVIEW_CHANGE_RECURRENCE'
export interface IChangeRecurrence {
    type: typeof RECURRENCESVIEW_CHANGE_RECURRENCE
    plannedRecurrence: PlannedRecurrence
}

export const RECURRENCESVIEW_SAVING_PROCESSING = 'RECURRENCESVIEW_SAVING_PROCESSING'
export interface ISaveRecurrencesProcessing {
    type: typeof RECURRENCESVIEW_SAVING_PROCESSING
}

export const RECURRENCESVIEW_SAVING_FINISH = 'RECURRENCESVIEW_SAVING_FINISH'
export interface ISaveRecurrencesFinish {
    type: typeof RECURRENCESVIEW_SAVING_FINISH
}

export const RECURRENCESVIEW_ADD_RECURRENCE = 'RECURRENCESVIEW_ADD_RECURRENCE'
export interface IAddRecurrence {
    type: typeof RECURRENCESVIEW_ADD_RECURRENCE
}

export const RECURRENCESVIEW_DELETE_RECURRENCE = 'RECURRENCESVIEW_DELETE_RECURRENCE'
export interface IDeleteRecurrence {
    type: typeof RECURRENCESVIEW_DELETE_RECURRENCE
    uid: string
}

export type RecurrencesViewAction =
    ICreateRecurrencesProcessing |
    ICreateRecurrencesFinish |
    ILoadRecurrencesProcessing |
    ILoadRecurrencesFail |
    ILoadRecurrencesSuccess |
    IChangeEdittingRecurrence |
    IChangeRecurrence |
    ISaveRecurrencesProcessing |
    ISaveRecurrencesFinish |
    IAddRecurrence |
    IDeleteRecurrence
