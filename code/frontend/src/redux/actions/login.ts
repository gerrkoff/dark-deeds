import { push as navigateTo, RouterAction } from 'connected-react-router'
import { loginApi } from 'di/api/login-api'
import { storageService } from 'di/services/storage-service'
import { toastService } from 'di/services/toast-service'
import { ThunkDispatch } from 'helpers'
import { SigninResultEnum, SignupResultEnum } from 'models'
import { Dispatch } from 'redux'
import { changeAllTasks } from 'redux/actions/tasks'
import * as actions from 'redux/constants'

export function initialLogin() {
    return async (dispatch: ThunkDispatch<actions.LoginAction>) => {
        const token = storageService.loadAccessToken()
        if (token === null || token === '') {
            return
        }

        dispatch(setInitialLogginIn(true))
        try {
            await loadCurrentUser(dispatch)
        } catch {
            toastService.errorProcess('login')
        }

        dispatch(setInitialLogginIn(false))
    }
}

export function signin(username: string, password: string) {
    return async (dispatch: ThunkDispatch<actions.LoginAction>) => {
        dispatch(processing())

        let result: SigninResultEnum
        try {
            const apiResult = await loginApi.signin(username, password)
            result = apiResult.result

            if (result === SigninResultEnum.Success) {
                storageService.saveAccessToken(apiResult.token)
                await loadCurrentUser(dispatch)
            }
        } catch (err) {
            result = SigninResultEnum.Unknown
            toastService.errorProcess('signin')
        }

        dispatch(signinResult(result))
    }
}

export function signup(username: string, password: string) {
    return async (dispatch: ThunkDispatch<actions.LoginAction>) => {
        dispatch(processing())

        let result: SignupResultEnum
        try {
            const apiResult = await loginApi.signup(username, password)
            result = apiResult.result

            if (result === SignupResultEnum.Success) {
                storageService.saveAccessToken(apiResult.token)
                await loadCurrentUser(dispatch)
            }
        } catch (err) {
            result = SignupResultEnum.Unknown
            toastService.errorProcess('signup')
        }

        dispatch(signupResult(result))
    }
}

export function signout() {
    return async (
        dispatch: ThunkDispatch<
            actions.LoginAction | RouterAction | actions.TasksAction
        >
    ) => {
        dispatch(changeAllTasks([]))
        storageService.clearAccessToken()
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

function setInitialLogginIn(
    initialLogginIn: boolean
): actions.ILoginInitialLogginIn {
    return { type: actions.LOGIN_INITIAL_LOGGING_IN, initialLogginIn }
}

function signinResult(result: SigninResultEnum): actions.ILoginSigninFinish {
    return { type: actions.LOGIN_SIGNIN_FINISH, result }
}

function signupResult(result: SignupResultEnum): actions.ILoginSignupFinish {
    return { type: actions.LOGIN_SIGNUP_FINISH, result }
}

function currentUser(
    userAuthenticated: boolean,
    userName?: string
): actions.ILoginCurrentUser {
    return { type: actions.LOGIN_CURRENT_USER, userAuthenticated, userName }
}

async function loadCurrentUser(dispatch: Dispatch<actions.LoginAction>) {
    const currentUserResult = await loginApi.current()
    dispatch(
        currentUser(
            currentUserResult.userAuthenticated,
            currentUserResult.username
        )
    )
}
