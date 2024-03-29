import * as actions from 'redux/constants'
import { IGeneralState } from 'redux/types'

const inittialState: IGeneralState = {
    appVersion: '',
}

export function general(
    state: IGeneralState = inittialState,
    action: actions.GeneralAction
): IGeneralState {
    switch (action.type) {
        case actions.GENERAL_UPDATE_BUILD_INFO:
            return { ...state, appVersion: action.appVersion }
    }
    return state
}
