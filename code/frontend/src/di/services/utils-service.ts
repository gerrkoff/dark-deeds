import { v4 as uuidv4 } from 'uuid'

export class UtilsService {
    public delay(waitTime: number): Promise<void> {
        return new Promise(resolve => {
            setTimeout(resolve, waitTime)
        })
    }

    public uuidv4(): string {
        return uuidv4()
    }
}

export const utilsService = new UtilsService()
