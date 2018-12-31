import { BuildInfo } from '../models'
import { Api } from './api'

const service = {
    loadBuildInfo(): Promise<BuildInfo> {
        return Api.get<BuildInfo>('api/build-info')
    }
}

export { service as GeneralApi }
