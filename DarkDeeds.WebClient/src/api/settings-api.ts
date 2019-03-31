import { Settings } from '../models'
import { Api } from './api'

const service = {
    load(): Promise<Settings> {
        return Api.get<Settings>('api/settings')
    },

    save(settings: Settings): Promise<void> {
        return Api.post('api/settings', settings)
    }
}

export { service as SettingsApi }
