import { baseUrlProvider, BaseUrlProvider } from '../../common/api/BaseUrlProvider'
import { storageService, StorageService } from '../../common/services/StorageService'
import { OAuthAuthorizeRequest } from '../models/OAuthAuthorizeRequest'
import { OAuthAuthorizeResult } from '../models/OAuthAuthorizeResult'

export class OAuthApi {
    constructor(
        private baseUrlProvider: BaseUrlProvider,
        private storageService: StorageService,
    ) {}

    async authorize(action: 'allow' | 'deny', request: OAuthAuthorizeRequest): Promise<OAuthAuthorizeResult> {
        try {
            const response = await fetch(`${this.baseUrlProvider.getBaseUrl()}authorize`, {
                body: JSON.stringify({
                    action,
                    clientId: request.clientId,
                    redirectUri: request.redirectUri,
                    codeChallenge: request.codeChallenge,
                    state: request.state,
                }),
                headers: {
                    Authorization: 'Bearer ' + this.storageService.loadAccessToken(),
                    'Content-Type': 'application/json',
                },
                method: 'POST',
            })

            if (response.status === 401) {
                return { status: 'needs-login' }
            }

            if (!response.ok) {
                return { status: 'error' }
            }

            const body = (await response.json()) as { redirectUrl: string }
            return { status: 'redirect', redirectUrl: body.redirectUrl }
        } catch {
            return { status: 'error' }
        }
    }
}

export const oauthApi = new OAuthApi(baseUrlProvider, storageService)
