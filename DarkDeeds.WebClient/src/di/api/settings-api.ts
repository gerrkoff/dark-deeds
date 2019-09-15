import { injectable } from 'inversify'
import { Api } from '..'
import { Settings } from '../../models'

@injectable()
export class SettingsApi {

    public constructor(
        private api: Api
    ) {}

    public load(): Promise<Settings> {
        return this.api.get<Settings>('api/settings')
    }

    public save(settings: Settings): Promise<void> {
        return this.api.post('api/settings', settings)
    }
}
