import { BuildInfoEntity } from '../models/BuildInfoEntity'
import { api, Api } from './Api'

export class GeneralApi {
    constructor(private api: Api) {}

    loadBuildInfo(): Promise<BuildInfoEntity> {
        return this.api.get<BuildInfoEntity>('api/be/build-info')
    }
}

export const generalApi = new GeneralApi(api)
