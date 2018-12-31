import { GeneralAction } from '../actions'
import { GENERAL_UPDATE_BUILD_INFO } from '../constants'
import { IGeneralState } from '../types'

const inittialState: IGeneralState = {
    appVersion: ''
}

export function general(state: IGeneralState = inittialState, action: GeneralAction): IGeneralState {
    switch (action.type) {
        case GENERAL_UPDATE_BUILD_INFO:
            return { ...state,
                appVersion: action.appVersion
            }
    }
    return state
}
