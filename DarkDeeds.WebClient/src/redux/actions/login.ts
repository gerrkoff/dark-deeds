import { Dispatch } from 'redux'
import { LoginApi } from '../../api'
import { StorageHelper, ToastHelper } from '../../helpers'
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
    initialLogginIn: boolean
}

export interface ILoginCurrentUser {
    type: constants.LOGIN_CURRENT_USER
    userAuthenticated: boolean
    userName?: string
}

export type LoginAction = ILoginProcessing | ILoginSigninFinish | ILoginInitialLogginIn | ILoginCurrentUser

export function initialLogin() {
    return async(dispatch: Dispatch<LoginAction>) => {
        dispatch(setInitialLogginIn(true))
        try {
            const currentUserResult = await LoginApi.current()
            dispatch(currentUser(currentUserResult.userAuthenticated, currentUserResult.username))
        } catch {
            ToastHelper.error(`Error occured while login`)
        }

        dispatch(setInitialLogginIn(false))
    }
}

export function signin(username: string, password: string) {
    return async(dispatch: Dispatch<LoginAction>) => {
        dispatch(processing())

        let result: SigninResultEnum
        try {
            const apiResult = await LoginApi.signin(username, password)
            result = apiResult.result

            if (result === SigninResultEnum.Success) {
                StorageHelper.Save(StorageHelper.TokenKey, apiResult.token)
                const currentUserResult = await LoginApi.current()
                dispatch(currentUser(currentUserResult.userAuthenticated, currentUserResult.username))
            }
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

function setInitialLogginIn(initialLogginIn: boolean): ILoginInitialLogginIn {
    return { type: constants.LOGIN_INITIAL_LOGGING_IN, initialLogginIn }
}

function signinResult(result: SigninResultEnum): ILoginSigninFinish {
    return { type: constants.LOGIN_SIGNIN_FINISH, result }
}

function currentUser(userAuthenticated: boolean, userName?: string): ILoginCurrentUser {
    return { type: constants.LOGIN_CURRENT_USER, userAuthenticated, userName }
}
