import { BuildInfo } from 'models'
import { Api, api } from 'di/api/api'

export class GeneralApi {
    public constructor(private api: Api) {}

    public loadBuildInfo(): Promise<BuildInfo> {
        return this.api.get<BuildInfo>('api/be/build-info')
    }
}

export const generalApi = new GeneralApi(api)
