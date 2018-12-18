import { SigninResult } from '../models'
import { Api } from './api'

const service = {
    signin(username: string, password: string): Promise<SigninResult> {
        return Api.post<SigninResult>('api/account/login', { username, password })
    }
}

export { service as LoginApi }
