import { SigninResultEnum, SignupResultEnum } from '../../models'

export interface ILoginState {
    processing: boolean
    userAuthenticated: boolean
    userName: string
    signinResult: SigninResultEnum
    signupResult: SignupResultEnum
    formSignin: boolean
    initialLogginIn: boolean
}
