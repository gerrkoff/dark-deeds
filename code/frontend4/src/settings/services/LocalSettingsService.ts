import {
    storageService,
    StorageService,
} from '../../common/services/StorageService'
import { SettingsLocalDto } from '../models/SettingsLocalDto'

export class LocalSettingsService {
    constructor(private storageService: StorageService) {}

    load(): SettingsLocalDto {
        const settingsSerialized = this.storageService.loadSettings()
        const defaultSettings: SettingsLocalDto = {
            overviewTabsExpanded: [],
            isDebugEnabled: false,
        }
        const settings: SettingsLocalDto =
            settingsSerialized === null
                ? defaultSettings
                : {
                      defaultSettings,
                      ...JSON.parse(settingsSerialized),
                  }

        return settings
    }

    save(settings: SettingsLocalDto) {
        const settingsSerialized = JSON.stringify(settings)
        this.storageService.saveSettings(settingsSerialized)
    }
}

export const localSettingsService = new LocalSettingsService(storageService)