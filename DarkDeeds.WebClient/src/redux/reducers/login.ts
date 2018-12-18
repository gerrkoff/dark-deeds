import { SigninResultEnum } from '../../models'
import { LoginAction } from '../actions'
import { LOGIN_INITIAL_LOGGING_IN, LOGIN_PROCESSING, LOGIN_SIGNIN_FINISH } from '../constants'
import { ILoginState } from '../types'

const initialState: ILoginState = {
    initialLogginIn: false,
    processing: false,
    signinResult: SigninResultEnum.Unknown,
    userAuthenticated: false,
    userName: ''
}

export function login(state: ILoginState = initialState, action: LoginAction): ILoginState {
    switch (action.type) {
        case LOGIN_PROCESSING:
            return { ...state,
                processing: true
            }
        case LOGIN_INITIAL_LOGGING_IN:
            return { ...state,
                initialLogginIn: true
            }
        case LOGIN_SIGNIN_FINISH:
            return { ...state,
                initialLogginIn: false,
                processing: false,
                signinResult: action.result,
                userAuthenticated: action.result === SigninResultEnum.Success
            }
    }
    return state
}
