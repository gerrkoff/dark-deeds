import { injectable } from 'inversify'

@injectable()
export class UtilsService {
    public delay(waitTime: number): Promise<void> {
        return new Promise(resolve => {
            setTimeout(resolve, waitTime)
        })
    }
}
