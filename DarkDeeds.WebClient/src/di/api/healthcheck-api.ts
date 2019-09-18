import { injectable, inject } from 'inversify'
import { Api } from '..'
import diToken from '../token'

@injectable()
export class HealthCheckApi {

    public constructor(
        @inject(diToken.Api) private api: Api
    ) {}

    public check(): Promise<string> {
        return this.api.get<string>('healthcheck')
        // Starting
        // Healthy
        // Unhealthy
    }
}
