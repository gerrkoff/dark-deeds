import { SigninResultEnum } from 'models'

export class SigninResult {
    constructor(public token: string, public result: SigninResultEnum) {}
}
