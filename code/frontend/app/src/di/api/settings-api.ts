import { SettingsServer } from '../../models'
import { Api, api } from './api'

export class SettingsApi {
    public constructor(private api: Api) {}

    public load(): Promise<SettingsServer> {
        return this.api.get<SettingsServer>('api/web/settings')
    }

    public save(settings: SettingsServer): Promise<void> {
        return this.api.post('api/web/settings', settings)
    }
}

export const settingsApi = new SettingsApi(api)
