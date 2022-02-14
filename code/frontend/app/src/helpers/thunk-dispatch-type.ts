import { Action } from 'redux'
import { ThunkDispatch as OriginalThunkDispatch } from 'redux-thunk'
import { IAppState } from 'redux/types'

export type ThunkDispatch<T extends Action> = OriginalThunkDispatch<
    IAppState,
    void,
    T
>
