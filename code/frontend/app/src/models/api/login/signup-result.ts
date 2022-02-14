import { SignupResultEnum } from '../..'

export class SignupResult {
    constructor(public token: string, public result: SignupResultEnum) {}
}
