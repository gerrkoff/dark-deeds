import { injectable, inject } from 'inversify'
import { Api } from '..'
import diToken from '../token'
import { CurrentUserInfo, SigninResult, SignupResult } from '../../models'

@injectable()
export class LoginApi {

    public constructor(
        @inject(diToken.Api) private api: Api
    ) {}

    public current(): Promise<CurrentUserInfo> {
        return this.api.get<CurrentUserInfo>('web/api/account')
    }

    public signin(username: string, password: string): Promise<SigninResult> {
        return this.api.post<SigninResult>('web/api/account/signin', { username, password })
    }

    public signup(username: string, password: string): Promise<SignupResult> {
        return this.api.post<SignupResult>('web/api/account/signup', { username, password })
    }
}
