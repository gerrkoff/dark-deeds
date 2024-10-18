import { Api, api } from '../../common/api/Api'
import { SettingsSharedDto } from '../models/SettingsSharedDto'

export class SettingsApi {
    public constructor(private api: Api) {}

    public load(): Promise<SettingsSharedDto> {
        return this.api.get<SettingsSharedDto>('api/web/settings')
    }

    public save(settings: SettingsSharedDto): Promise<void> {
        return this.api.post('api/web/settings', settings)
    }
}

export const settingsApi = new SettingsApi(api)
