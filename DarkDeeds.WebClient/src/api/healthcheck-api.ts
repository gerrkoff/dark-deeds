import { Api } from './api'

const service = {
    check(): Promise<string> {
        return Api.get<string>('healthcheck')
    }
}

export { service as HealthCheckApi }
