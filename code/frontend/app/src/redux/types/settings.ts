import { AppearanceThemeEnum } from '../../models'

export interface ISettings {
    saveProcessing: boolean
    loadProcessing: boolean
    showCompleted: boolean
    appearanceTheme: AppearanceThemeEnum
}
