import { injectable, inject } from 'inversify'
import { Api } from '..'
import diToken from '../token'
import { SettingsServer } from '../../models'

@injectable()
export class SettingsApi {

    public constructor(
        @inject(diToken.Api) private api: Api
    ) {}

    public load(): Promise<SettingsServer> {
        return this.api.get<SettingsServer>('api/web/settings')
    }

    public save(settings: SettingsServer): Promise<void> {
        return this.api.post('api/web/settings', settings)
    }
}
