import { Dispatch } from 'redux'
import { LoginApi } from '../../api'
import { ToastHelper } from '../../helpers'
import { SigninResultEnum } from '../../models'
import * as constants from '../constants'

export interface ILoginProcessing {
    type: constants.LOGIN_PROCESSING
}

export interface ILoginSigninFinish {
    type: constants.LOGIN_SIGNIN_FINISH
    result: SigninResultEnum
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

export function signin(username: string, password: string) {
    return async(dispatch: Dispatch<LoginAction>) => {
        dispatch(processing())

        let result: SigninResultEnum
        try {
            result = (await LoginApi.signin(username, password)).result
            // TODO: save token
        } catch (err) {
            result = SigninResultEnum.Unknown
            ToastHelper.error(`Error occured while signin`)
        }
        dispatch(signinResult(result))
    }
}

function processing(): ILoginProcessing {
    return { type: constants.LOGIN_PROCESSING }
}

function initialLogginIn(): ILoginInitialLogginIn {
    return { type: constants.LOGIN_INITIAL_LOGGING_IN }
}

function signinResult(result: SigninResultEnum): ILoginSigninFinish {
    return { type: constants.LOGIN_SIGNIN_FINISH, result }
}
