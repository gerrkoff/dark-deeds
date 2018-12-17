import { Dispatch } from 'redux'
import { SigninResultEnum } from '../../models'
import * as constants from '../constants'

export interface ILoginProcessing {
    type: constants.LOGIN_PROCESSING
}

export interface ILoginSigninFinish {
    type: constants.LOGIN_SIGNIN_FINISH
    result: SigninResultEnum
    userName: string
}

export interface ILoginInitialLogginIn {
    type: constants.LOGIN_INITIAL_LOGGING_IN
}

export type LoginAction = ILoginProcessing | ILoginSigninFinish | ILoginInitialLogginIn

export function initialLogin() {
    return async(dispatch: Dispatch<LoginAction>) => {
        dispatch(initialLogginIn())
        setTimeout(() => dispatch(signinResult(SigninResultEnum.Unknown)), 2000)
    }
}

export function signin(login: string, password: string) {
    return async(dispatch: Dispatch<LoginAction>) => {
        dispatch(processing())
        setTimeout(() => dispatch(signinResult(SigninResultEnum.Success, 'Test')), 3000)
    }
}

function processing(): ILoginProcessing {
    return { type: constants.LOGIN_PROCESSING }
}

function initialLogginIn(): ILoginInitialLogginIn {
    return { type: constants.LOGIN_INITIAL_LOGGING_IN }
}

function signinResult(result: SigninResultEnum, userName: string = ''): ILoginSigninFinish {
    return { type: constants.LOGIN_SIGNIN_FINISH, result, userName }
}
