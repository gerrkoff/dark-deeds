import {
    LocalSettingsService,
    localSettingsService,
} from 'di/services/local-settings-service'
import { AppearanceThemeEnum } from 'models'

export class AppearanceService {
    public constructor(private localSettingsService: LocalSettingsService) {}

    public initTheme() {
        const theme = this.localSettingsService.load().appearanceTheme
        this.applyTheme(theme)
    }

    public applyTheme(theme: AppearanceThemeEnum) {
        const html = document.querySelector('html')
        if (html === null) {
            return
        }

        if (theme === AppearanceThemeEnum.Dark) {
            html.style.setProperty('--clr-base', '#555')
            html.style.setProperty('--clr-main', '#131313')
            html.style.setProperty('--clr-help', '#323232')
            html.style.setProperty('--clr-help-2', '#444446')
            html.style.setProperty('--clr-text-primary', '#fdfdfd')
            html.style.setProperty('--clr-text-secondary', '#646464')
            html.style.setProperty('--clr-shadow', '#353A45')
            html.style.setProperty('--clr-success', '#058909')
            html.style.setProperty('--clr-error', '#B43B2F')
            html.style.setProperty('--clr-warn', '#BE9B0C')
            html.style.setProperty('--clr-info', '#2875A8')
        } else if (theme === AppearanceThemeEnum.Light) {
            html.style.setProperty('--clr-base', '#f2f3f4')
            html.style.setProperty('--clr-main', 'white')
            html.style.setProperty('--clr-help', 'gainsboro')
            html.style.setProperty('--clr-help-2', 'lightgray')
            html.style.setProperty('--clr-text-primary', 'black')
            html.style.setProperty('--clr-text-secondary', '#a2a2a2')
            html.style.setProperty('--clr-shadow', '#c5c6c7')
            html.style.setProperty('--clr-success', '#058909')
            html.style.setProperty('--clr-error', '#B43B2F')
            html.style.setProperty('--clr-warn', '#BE9B0C')
            html.style.setProperty('--clr-info', '#2875A8')
        } else if (theme === AppearanceThemeEnum.Vk) {
            html.style.setProperty('--clr-base', '#EDEEF0')
            html.style.setProperty('--clr-main', '#FFF')
            html.style.setProperty('--clr-help', '#D2D8DF')
            html.style.setProperty('--clr-help-2', '#E1E5EB')
            html.style.setProperty('--clr-text-primary', '#55677d')
            html.style.setProperty('--clr-text-secondary', '#828282')
            html.style.setProperty('--clr-shadow', '#dfe2e8')
            html.style.setProperty('--clr-success', '#058909')
            html.style.setProperty('--clr-error', '#B43B2F')
            html.style.setProperty('--clr-warn', '#BE9B0C')
            html.style.setProperty('--clr-info', '#2875A8')
        } else if (theme === AppearanceThemeEnum.Dolls) {
            html.style.setProperty('--clr-base', '#f6d55c')
            html.style.setProperty('--clr-main', '#3caea3')
            html.style.setProperty('--clr-help', '#f4a460')
            html.style.setProperty('--clr-help-2', '#ed553b')
            html.style.setProperty('--clr-text-primary', '#131f1e')
            html.style.setProperty('--clr-text-secondary', '#2e716a')
            html.style.setProperty('--clr-shadow', '#c9ae4d')
            html.style.setProperty('--clr-success', '#058909')
            html.style.setProperty('--clr-error', '#B43B2F')
            html.style.setProperty('--clr-warn', '#BE9B0C')
            html.style.setProperty('--clr-info', '#2875A8')
        } else if (theme === AppearanceThemeEnum.Nature) {
            html.style.setProperty('--clr-base', '#7da3a1')
            html.style.setProperty('--clr-main', '#324851')
            html.style.setProperty('--clr-help', '#34675c')
            html.style.setProperty('--clr-help-2', '#116062')
            html.style.setProperty('--clr-text-primary', '#EDF0EA')
            html.style.setProperty('--clr-text-secondary', '#728087')
            html.style.setProperty('--clr-shadow', '#678684')
            html.style.setProperty('--clr-success', '#058909')
            html.style.setProperty('--clr-error', '#B43B2F')
            html.style.setProperty('--clr-warn', '#BE9B0C')
            html.style.setProperty('--clr-info', '#2875A8')
        }
    }
}

export const appearanceService = new AppearanceService(localSettingsService)
