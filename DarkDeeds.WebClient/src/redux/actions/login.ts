import { Dispatch } from 'redux'
import { push as navigateTo, RouterAction } from 'connected-react-router'
import { LoginApi } from '../../api'
import { StorageService, ToastService } from '../../services'
import { SigninResultEnum, SignupResultEnum } from '../../models'
import * as c from '../constants'

export function initialLogin() {
    return async(dispatch: Dispatch<c.LoginAction>) => {
        const token = StorageService.loadAccessToken()
        if (token === null || token === '') {
            return
        }

        dispatch(setInitialLogginIn(true))
        try {
            await loadCurrentUser(dispatch)
        } catch {
            ToastService.errorProcess('login')
        }

        dispatch(setInitialLogginIn(false))
    }
}

export function signin(username: string, password: string) {
    return async(dispatch: Dispatch<c.LoginAction>) => {
        dispatch(processing())

        let result: SigninResultEnum
        try {
            const apiResult = await LoginApi.signin(username, password)
            result = apiResult.result

            if (result === SigninResultEnum.Success) {
                StorageService.saveAccessToken(apiResult.token)
                await loadCurrentUser(dispatch)
            }
        } catch (err) {
            result = SigninResultEnum.Unknown
            ToastService.errorProcess('signin')
        }

        dispatch(signinResult(result))
    }
}

export function signup(username: string, password: string) {
    return async(dispatch: Dispatch<c.LoginAction>) => {
        dispatch(processing())

        let result: SignupResultEnum
        try {
            const apiResult = await LoginApi.signup(username, password)
            result = apiResult.result

            if (result === SignupResultEnum.Success) {
                StorageService.saveAccessToken(apiResult.token)
                await loadCurrentUser(dispatch)
            }
        } catch (err) {
            result = SignupResultEnum.Unknown
            ToastService.errorProcess('signup')
        }

        dispatch(signupResult(result))
    }
}

export function signout() {
    return async(dispatch: Dispatch<c.LoginAction | RouterAction>) => {
        StorageService.clearAccessToken()
        dispatch(navigateTo('/'))
        dispatch(currentUser(false))
    }
}

export function switchForm(formSignin: boolean): c.ILoginSwitchForm {
    return { type: c.LOGIN_SWITCH_FORM, formSignin }
}

function processing(): c.ILoginProcessing {
    return { type: c.LOGIN_PROCESSING }
}

function setInitialLogginIn(initialLogginIn: boolean): c.ILoginInitialLogginIn {
    return { type: c.LOGIN_INITIAL_LOGGING_IN, initialLogginIn }
}

function signinResult(result: SigninResultEnum): c.ILoginSigninFinish {
    return { type: c.LOGIN_SIGNIN_FINISH, result }
}

function signupResult(result: SignupResultEnum): c.ILoginSignupFinish {
    return { type: c.LOGIN_SIGNUP_FINISH, result }
}

function currentUser(userAuthenticated: boolean, userName?: string): c.ILoginCurrentUser {
    return { type: c.LOGIN_CURRENT_USER, userAuthenticated, userName }
}

async function loadCurrentUser(dispatch: Dispatch<c.LoginAction>) {
    const currentUserResult = await LoginApi.current()
    dispatch(currentUser(currentUserResult.userAuthenticated, currentUserResult.username))
}
