import { Api } from './api'

const service = {
    check(): Promise<string> {
        return Api.get<string>('healthcheck')
        // Starting
        // Healthy
        // Unhealthy
    }
}

export { service as HealthCheckApi }
