import { Action } from 'redux'
import { IAppState } from 'redux/types'
import { ThunkDispatch as OriginalThunkDispatch } from 'redux-thunk'

export type ThunkDispatch<T extends Action> = OriginalThunkDispatch<
    IAppState,
    void,
    T
>
