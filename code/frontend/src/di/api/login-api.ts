import { Api, api } from 'di/api/api'
import { CurrentUserInfo, SigninResult, SignupResult } from 'models'

export class LoginApi {
    public constructor(private api: Api) {}

    public current(): Promise<CurrentUserInfo> {
        return this.api.get<CurrentUserInfo>('api/auth/account')
    }

    public signin(username: string, password: string): Promise<SigninResult> {
        return this.api.post<SigninResult>('api/auth/account/signin', {
            username,
            password,
        })
    }

    public signup(username: string, password: string): Promise<SignupResult> {
        return this.api.post<SignupResult>('api/auth/account/signup', {
            username,
            password,
        })
    }
}

export const loginApi = new LoginApi(api)
