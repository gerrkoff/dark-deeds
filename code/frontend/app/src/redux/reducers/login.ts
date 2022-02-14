import { SigninResultEnum, SignupResultEnum } from '../../models'
import { ILoginState } from '../types'
import * as actions from '../constants'

const initialState: ILoginState = {
    formSignin: true,
    initialLogginIn: false,
    processing: false,
    signinResult: SigninResultEnum.Unknown,
    signupResult: SignupResultEnum.Unknown,
    userAuthenticated: false,
    userName: '',
}

export function login(
    state: ILoginState = initialState,
    action: actions.LoginAction
): ILoginState {
    switch (action.type) {
        case actions.LOGIN_INITIAL_LOGGING_IN:
            return { ...state, initialLogginIn: action.initialLogginIn }
        case actions.LOGIN_CURRENT_USER:
            return {
                ...state,
                userAuthenticated: action.userAuthenticated,
                userName: action.userName ? action.userName : '',
            }
        case actions.LOGIN_PROCESSING:
            return { ...state, processing: true }
        case actions.LOGIN_SIGNIN_FINISH:
            return { ...state, processing: false, signinResult: action.result }
        case actions.LOGIN_SIGNUP_FINISH:
            return { ...state, processing: false, signupResult: action.result }
        case actions.LOGIN_SWITCH_FORM:
            return {
                ...state,
                formSignin: action.formSignin,
                signinResult: SigninResultEnum.Unknown,
                signupResult: SignupResultEnum.Unknown,
            }
    }
    return state
}
