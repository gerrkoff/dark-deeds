import { SignupResultEnum } from 'models'

export class SignupResult {
    constructor(public token: string, public result: SignupResultEnum) {}
}
