import { injectable, inject } from 'inversify'
import { Api } from '..'
import diToken from '../token'
import { BuildInfo } from '../../models'

@injectable()
export class GeneralApi {

    public constructor(
        @inject(diToken.Api) private api: Api
    ) {}

    public loadBuildInfo(): Promise<BuildInfo> {
        return this.api.get<BuildInfo>('api/be/build-info')
    }
}
