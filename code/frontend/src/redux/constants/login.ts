import { SigninResultEnum, SignupResultEnum } from 'models'

export const LOGIN_PROCESSING = 'LOGIN_PROCESSING'
export interface ILoginProcessing {
    type: typeof LOGIN_PROCESSING
}

export const LOGIN_SIGNIN_FINISH = 'LOGIN_SIGNIN_FINISH'
export interface ILoginSigninFinish {
    type: typeof LOGIN_SIGNIN_FINISH
    result: SigninResultEnum
}

export const LOGIN_SIGNUP_FINISH = 'LOGIN_SIGNUP_FINISH'
export interface ILoginSignupFinish {
    type: typeof LOGIN_SIGNUP_FINISH
    result: SignupResultEnum
}

export const LOGIN_INITIAL_LOGGING_IN = 'LOGIN_INITIAL_LOGGING_IN'
export interface ILoginInitialLogginIn {
    type: typeof LOGIN_INITIAL_LOGGING_IN
    initialLogginIn: boolean
}

export const LOGIN_CURRENT_USER = 'LOGIN_CURRENT_USER'
export interface ILoginCurrentUser {
    type: typeof LOGIN_CURRENT_USER
    userAuthenticated: boolean
    userName?: string
}

export const LOGIN_SWITCH_FORM = 'LOGIN_SWITCH_FORM'
export interface ILoginSwitchForm {
    type: typeof LOGIN_SWITCH_FORM
    formSignin: boolean
}

export type LoginAction =
    | ILoginProcessing
    | ILoginSigninFinish
    | ILoginSignupFinish
    | ILoginInitialLogginIn
    | ILoginCurrentUser
    | ILoginSwitchForm
