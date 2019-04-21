import { ToastHelper } from './'
import { HealthCheckApi } from '../api'

const service = {
    async ping() {
        const result = (await HealthCheckApi.check()).toLowerCase()
        console.log('Healhcheck:', result)

        if (result === checkValues.Unhealthy) {
            ToastHelper.info('There are some problems with connection')
        }
    }
}

const checkValues = {
    Starting: 'starting',
    Healthy: 'healthy',
    Unhealthy: 'unhealthy'
}

export { service as HealthChecker }
