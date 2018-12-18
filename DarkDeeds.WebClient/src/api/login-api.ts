import { CurrentUserInfo, SigninResult } from '../models'
import { Api } from './api'

const service = {
    current(): Promise<CurrentUserInfo> {
        return Api.get<CurrentUserInfo>('api/account')
    },

    signin(username: string, password: string): Promise<SigninResult> {
        return Api.post<SigninResult>('api/account/signin', { username, password })
    }
}

export { service as LoginApi }
