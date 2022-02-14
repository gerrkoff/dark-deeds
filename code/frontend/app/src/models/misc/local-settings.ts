import { OverviewTabEnum, AppearanceThemeEnum } from '..'

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
