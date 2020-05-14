import { injectable, inject } from 'inversify'
import { AppearanceThemeEnum } from '../../models'
import { LocalSettingsService } from '..'
import diToken from '../token'

@injectable()
export class AppearanceService {
    public constructor(
        @inject(diToken.LocalSettingsService) private localSettingsService: LocalSettingsService
    ) {}

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
        } else {
            html.style.setProperty('--clr-base', '#f2f3f4')
            html.style.setProperty('--clr-main', 'white')
            html.style.setProperty('--clr-help', 'gainsboro')
            html.style.setProperty('--clr-help-2', 'lightgray')
            html.style.setProperty('--clr-text-primary', 'black')
            html.style.setProperty('--clr-text-secondary', 'dimgray')
            html.style.setProperty('--clr-shadow', 'rgba(34,36,38,.12)')
            html.style.setProperty('--clr-success', '#058909')
            html.style.setProperty('--clr-error', '#B43B2F')
            html.style.setProperty('--clr-warn', '#BE9B0C')
            html.style.setProperty('--clr-info', '#2875A8')
        }
    }
}
