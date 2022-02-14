import { SigninResultEnum } from '../..'

export class SigninResult {
    constructor(public token: string, public result: SigninResultEnum) {}
}
