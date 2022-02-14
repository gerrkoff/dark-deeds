export class UtilsService {
    public delay(waitTime: number): Promise<void> {
        return new Promise(resolve => {
            setTimeout(resolve, waitTime)
        })
    }

    public uuidv4(): string {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
            const r = (Math.random() * 16) | 0
            const v = c === 'x' ? r : (r & 0x3) | 0x8
            return v.toString(16)
        })
    }
}

export const utilsService = new UtilsService()
