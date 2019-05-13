import { Dispatch } from 'redux'
import { push as navigateTo, RouterAction } from 'connected-react-router'
import { LoginApi } from '../../api'
import { StorageService, ToastService } from '../../services'
import { SigninResultEnum, SignupResultEnum } from '../../models'
import * as actions from '../constants/login'

export function initialLogin() {
    return async(dispatch: Dispatch<actions.LoginAction>) => {
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
    return async(dispatch: Dispatch<actions.LoginAction>) => {
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
    return async(dispatch: Dispatch<actions.LoginAction>) => {
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
    return async(dispatch: Dispatch<actions.LoginAction | RouterAction>) => {
        StorageService.clearAccessToken()
        dispatch(navigateTo('/'))
        dispatch(currentUser(false))
    }
}

export function switchForm(formSignin: boolean): actions.ILoginSwitchForm {
    return { type: actions.LOGIN_SWITCH_FORM, formSignin }
}

function processing(): actions.ILoginProcessing {
    return { type: actions.LOGIN_PROCESSING }
}

function setInitialLogginIn(initialLogginIn: boolean): actions.ILoginInitialLogginIn {
    return { type: actions.LOGIN_INITIAL_LOGGING_IN, initialLogginIn }
}

function signinResult(result: SigninResultEnum): actions.ILoginSigninFinish {
    return { type: actions.LOGIN_SIGNIN_FINISH, result }
}

function signupResult(result: SignupResultEnum): actions.ILoginSignupFinish {
    return { type: actions.LOGIN_SIGNUP_FINISH, result }
}

function currentUser(userAuthenticated: boolean, userName?: string): actions.ILoginCurrentUser {
    return { type: actions.LOGIN_CURRENT_USER, userAuthenticated, userName }
}

async function loadCurrentUser(dispatch: Dispatch<actions.LoginAction>) {
    const currentUserResult = await LoginApi.current()
    dispatch(currentUser(currentUserResult.userAuthenticated, currentUserResult.username))
}
