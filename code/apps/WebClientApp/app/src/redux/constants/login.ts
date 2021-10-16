import { SigninResultEnum, SignupResultEnum } from '../../models'

export const LOGIN_PROCESSING = 'LOGIN_PROCESSING'
export type LOGIN_PROCESSING = typeof LOGIN_PROCESSING
export interface ILoginProcessing {
    type: LOGIN_PROCESSING
}

export const LOGIN_SIGNIN_FINISH = 'LOGIN_SIGNIN_FINISH'
export type LOGIN_SIGNIN_FINISH = typeof LOGIN_SIGNIN_FINISH
export interface ILoginSigninFinish {
    type: LOGIN_SIGNIN_FINISH
    result: SigninResultEnum
}

export const LOGIN_SIGNUP_FINISH = 'LOGIN_SIGNUP_FINISH'
export type LOGIN_SIGNUP_FINISH = typeof LOGIN_SIGNUP_FINISH
export interface ILoginSignupFinish {
    type: LOGIN_SIGNUP_FINISH
    result: SignupResultEnum
}

export const LOGIN_INITIAL_LOGGING_IN = 'LOGIN_INITIAL_LOGGING_IN'
export type LOGIN_INITIAL_LOGGING_IN = typeof LOGIN_INITIAL_LOGGING_IN
export interface ILoginInitialLogginIn {
    type: LOGIN_INITIAL_LOGGING_IN
    initialLogginIn: boolean
}

export const LOGIN_CURRENT_USER = 'LOGIN_CURRENT_USER'
export type LOGIN_CURRENT_USER = typeof LOGIN_CURRENT_USER
export interface ILoginCurrentUser {
    type: LOGIN_CURRENT_USER
    userAuthenticated: boolean
    userName?: string
}

export const LOGIN_SWITCH_FORM = 'LOGIN_SWITCH_FORM'
export type LOGIN_SWITCH_FORM = typeof LOGIN_SWITCH_FORM
export interface ILoginSwitchForm {
    type: LOGIN_SWITCH_FORM
    formSignin: boolean
}

export type LoginAction =
    ILoginProcessing |
    ILoginSigninFinish |
    ILoginSignupFinish |
    ILoginInitialLogginIn |
    ILoginCurrentUser |
    ILoginSwitchForm
