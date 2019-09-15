import { injectable, inject } from 'inversify'
import { Api } from '..'
import service from '../service'
import { CurrentUserInfo, SigninResult, SignupResult } from '../../models'

@injectable()
export class LoginApi {

    public constructor(
        @inject(service.Api) private api: Api
    ) {}

    public current(): Promise<CurrentUserInfo> {
        return this.api.get<CurrentUserInfo>('api/account')
    }

    public signin(username: string, password: string): Promise<SigninResult> {
        return this.api.post<SigninResult>('api/account/signin', { username, password })
    }

    public signup(username: string, password: string): Promise<SignupResult> {
        return this.api.post<SignupResult>('api/account/signup', { username, password })
    }
}
