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
        return this.api.post<SigninResultDto>('api/auth/account/signin', {
            username,
            password,
        })
    }

    signup(username: string, password: string): Promise<SignupResultDto> {
        return this.api.post<SignupResultDto>('api/auth/account/signup', {
            username,
            password,
        })
    }
}

export const loginApi = new LoginApi(api)