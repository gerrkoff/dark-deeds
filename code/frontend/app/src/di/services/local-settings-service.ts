import { LocalSettings } from '../../models'
import { StorageService, storageService } from './storage-service'

export class LocalSettingsService {
    private settingsInitialized = false
    private settings: LocalSettings | null = null

    public constructor(private storageService: StorageService) {}

    public load(): LocalSettings {
        if (!this.settingsInitialized) {
            this.settingsInitialized = true
            const settingsSerialized = this.storageService.loadSettings()
            this.settings =
                settingsSerialized === null
                    ? new LocalSettings()
                    : {
                          ...new LocalSettings(),
                          ...JSON.parse(settingsSerialized),
                      }
        }
        return this.settings!
    }

    public save() {
        const settingsSerialized = JSON.stringify(this.settings)
        this.storageService.saveSettings(settingsSerialized)
    }
}

export const localSettingsService = new LocalSettingsService(storageService)
