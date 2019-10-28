import { injectable } from 'inversify'
// import { injectable, inject } from 'inversify'
// import { Api } from '..'
// import diToken from '../token'

@injectable()
export class RecurrencesViewApi {

    // public constructor(
    //     @inject(diToken.Api) private api: Api
    // ) {}

    public createRecurrences(): Promise<void> {
        return new Promise((x, y) => setTimeout(y, 3000))
    }
}
