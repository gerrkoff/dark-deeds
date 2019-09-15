import { injectable } from 'inversify'
import { LocalSettings } from '../../models'
import { StorageService } from '..'

@injectable()
export class LocalSettingsService {
    private settingsInitialized = false
    private settings: LocalSettings

    public constructor(
        private storageService: StorageService
    ) {}

    public load(): LocalSettings {
        if (!this.settingsInitialized) {
            this.settingsInitialized = true
            const settingsSerialized = this.storageService.loadSettings()
            this.settings = settingsSerialized === null
                ? new LocalSettings()
                : JSON.parse(settingsSerialized)
        }
        return this.settings
    }

    public save() {
        const settingsSerialized = JSON.stringify(this.settings)
        this.storageService.saveSettings(settingsSerialized)
    }
}
