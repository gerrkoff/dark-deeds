import { BuildInfoDto } from '../models/BuildInfoDto'
import { api, Api } from '../../common/api/Api'

export class GeneralApi {
    constructor(private api: Api) {}

    fetchBuildInfo(): Promise<BuildInfoDto> {
        return this.api.get<BuildInfoDto>('api/be/build-info')
    }
}

export const generalApi = new GeneralApi(api)
