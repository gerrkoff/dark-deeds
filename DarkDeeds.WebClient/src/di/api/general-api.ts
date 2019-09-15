import { injectable, inject } from 'inversify'
import { Api } from '..'
import service from '../service'
import { BuildInfo } from '../../models'

@injectable()
export class GeneralApi {

    public constructor(
        @inject(service.Api) private api: Api
    ) {}

    public loadBuildInfo(): Promise<BuildInfo> {
        return this.api.get<BuildInfo>('api/build-info')
    }
}
