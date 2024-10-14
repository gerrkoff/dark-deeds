import { BuildInfoEntity } from '../models/BuildInfoEntity'
import { api, Api } from './api'

export class GeneralApi {
    public constructor(private api: Api) {}

    public loadBuildInfo(): Promise<BuildInfoEntity> {
        return this.api.get<BuildInfoEntity>('api/be/build-info')
    }
}

export const generalApi = new GeneralApi(api)
