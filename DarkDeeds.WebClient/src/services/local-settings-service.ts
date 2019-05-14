import { LocalSettings } from '../models'
import { StorageService } from '.'

const service = {
    load: (): LocalSettings => {
        if (!settingsInitialized) {
            settingsInitialized = true
            const settingsSerialized = StorageService.loadSettings()
            settings = settingsSerialized === null
                ? new LocalSettings()
                : JSON.parse(settingsSerialized)
        }
        return settings
    },

    save: () => {
        const settingsSerialized = JSON.stringify(settings)
        StorageService.saveSettings(settingsSerialized)
    }
}

let settingsInitialized = false
let settings: LocalSettings

export { service as LocalSettingsService }
