import { CurrentUserInfo, SigninResult, SignupResult } from '../../models'
import { Api, api } from './api'

export class LoginApi {

    public constructor(
        private api: Api
    ) {}

    public current(): Promise<CurrentUserInfo> {
        return this.api.get<CurrentUserInfo>('api/web/account')
    }

    public signin(username: string, password: string): Promise<SigninResult> {
        return this.api.post<SigninResult>('api/web/account/signin', { username, password })
    }

    public signup(username: string, password: string): Promise<SignupResult> {
        return this.api.post<SignupResult>('api/web/account/signup', { username, password })
    }
}

export const loginApi = new LoginApi(api)
