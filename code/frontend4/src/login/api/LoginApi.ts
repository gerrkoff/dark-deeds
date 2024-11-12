import { api, Api } from '../../common/api/Api'
import { CurrentUserDto } from '../models/CurrentUserDto'
import { SigninResultDto } from '../models/SigninResultDto'
import { SignupResultDto } from '../models/SignupResultDto'

export class LoginApi {
    constructor(private api: Api) {}

    fetchCurrentUser(): Promise<CurrentUserDto> {
        return this.api.get<CurrentUserDto>('api/auth/account')
    }

    signin(username: string, password: string): Promise<SigninResultDto> {
        return this.api.post<unknown, SigninResultDto>(
            'api/auth/account/signin',
            {
                username,
                password,
            },
        )
    }

    signup(username: string, password: string): Promise<SignupResultDto> {
        return this.api.post<unknown, SignupResultDto>(
            'api/auth/account/signup',
            {
                username,
                password,
            },
        )
    }

    renewToken(): Promise<string> {
        return this.api.post<unknown, string>('api/auth/account/renew', {})
    }
}

export const loginApi = new LoginApi(api)
