import { injectable, inject } from 'inversify'
import { Api } from '..'
import service from '../service'
import { Settings } from '../../models'

@injectable()
export class SettingsApi {

    public constructor(
        @inject(service.Api) private api: Api
    ) {}

    public load(): Promise<Settings> {
        return this.api.get<Settings>('api/settings')
    }

    public save(settings: Settings): Promise<void> {
        return this.api.post('api/settings', settings)
    }
}
