import { storageService, StorageService } from '../../common/services/StorageService'
import { UserModel } from '../models/UserModel'
import { parseJwt } from '../../common/utils/parseJwt'

export class AuthService {
    constructor(private storageService: StorageService) {}

    getCurrentUser(): UserModel | null {
        const jwtToken = this.storageService.loadAccessToken()

        if (!jwtToken) {
            return null
        }

        try {
            const payload = parseJwt<{
                aud: string
                exp: number
                given_name: string
                iat: number
                iss: string
                jti: string
                name: string
                nbf: number
                sub: string
            }>(jwtToken)

            return {
                username: payload.name,
                expiresAt: payload.exp * 1000,
            }
        } catch (error) {
            console.error(error)
            return null
        }
    }
}

export const authService = new AuthService(storageService)
