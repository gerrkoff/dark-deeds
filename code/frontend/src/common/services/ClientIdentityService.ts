import { uuidv4 } from '../utils/uuidv4'

export class ClientIdentityService {
    private readonly clientId: string

    constructor() {
        this.clientId = uuidv4()
    }

    getClientId(): string {
        return this.clientId
    }
}

export const clientIdentityService = new ClientIdentityService()
