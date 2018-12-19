import { SigninResultEnum, SignupResultEnum } from '../../models'
import { LoginAction } from '../actions'
import { LOGIN_CURRENT_USER, LOGIN_INITIAL_LOGGING_IN, LOGIN_PROCESSING, LOGIN_SIGNIN_FINISH, LOGIN_SIGNUP_FINISH } from '../constants'
import { ILoginState } from '../types'

const initialState: ILoginState = {
    formSignin: false,
    initialLogginIn: false,
    processing: false,
    signinResult: SigninResultEnum.Unknown,
    signupResult: SignupResultEnum.Unknown,
    userAuthenticated: false,
    userName: ''
}

export function login(state: ILoginState = initialState, action: LoginAction): ILoginState {
    switch (action.type) {
        case LOGIN_INITIAL_LOGGING_IN:
            return { ...state,
                initialLogginIn: action.initialLogginIn
            }
        case LOGIN_CURRENT_USER:
            return { ...state,
                userAuthenticated: action.userAuthenticated,
                userName: action.userName ? action.userName : ''
            }
        case LOGIN_PROCESSING:
            return { ...state,
                processing: true
            }
        case LOGIN_SIGNIN_FINISH:
            return { ...state,
                processing: false,
                signinResult: action.result
            }
        case LOGIN_SIGNUP_FINISH:
            return { ...state,
                processing: false,
                signupResult: action.result
            }
    }
    return state
}
