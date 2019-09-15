import { injectable, inject } from 'inversify'
import { Api } from '..'
import service from '../service'

@injectable()
export class HealthCheckApi {

    public constructor(
        @inject(service.Api) private api: Api
    ) {}

    public check(): Promise<string> {
        return this.api.get<string>('healthcheck')
        // Starting
        // Healthy
        // Unhealthy
    }
}
