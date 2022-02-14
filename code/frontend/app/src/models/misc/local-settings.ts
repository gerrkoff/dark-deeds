import { OverviewTabEnum, AppearanceThemeEnum } from 'models'

export class LocalSettings {
    constructor(
        public openedOverviewTabs: OverviewTabEnum[] = [
            OverviewTabEnum.NoDate,
            OverviewTabEnum.Expired,
            OverviewTabEnum.Current,
            OverviewTabEnum.Future,
        ],
        public appearanceTheme: AppearanceThemeEnum = AppearanceThemeEnum.Dark
    ) {}
}
