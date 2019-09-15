import { injectable } from 'inversify'
import { Api } from '..'
import { BuildInfo } from '../../models'

@injectable()
export class GeneralApi {

    public constructor(
        private api: Api
    ) {}

    public loadBuildInfo(): Promise<BuildInfo> {
        return this.api.get<BuildInfo>('api/build-info')
    }
}
