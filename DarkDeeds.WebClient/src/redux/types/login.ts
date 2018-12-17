import { SigninResultEnum } from '../../models'

export interface ILoginState {
    processing: boolean
    userAuthenticated: boolean
    userName: string
    signinResult: SigninResultEnum
    initialLogginIn: boolean
}
