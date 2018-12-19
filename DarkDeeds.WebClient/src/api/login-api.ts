import { CurrentUserInfo, SigninResult, SignupResult } from '../models'
import { Api } from './api'

const service = {
    current(): Promise<CurrentUserInfo> {
        return Api.get<CurrentUserInfo>('api/account')
    },

    signin(username: string, password: string): Promise<SigninResult> {
        return Api.post<SigninResult>('api/account/signin', { username, password })
    },

    signup(username: string, password: string): Promise<SignupResult> {
        return Api.post<SignupResult>('api/account/signup', { username, password })
    }
}

export { service as LoginApi }
