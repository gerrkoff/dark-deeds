import { Api, api } from './api'

export class HealthCheckApi {

    public constructor(
        private api: Api
    ) {}

    public check(): Promise<string> {
        return this.api.get<string>('healthcheck')
        // Starting
        // Healthy
        // Unhealthy
    }
}

export const healthCheckApi = new HealthCheckApi(api)
