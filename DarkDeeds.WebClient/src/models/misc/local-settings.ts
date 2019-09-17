import { OverviewTabEnum } from '..'

export class LocalSettings {
    constructor(
        public openedOverviewTabs: OverviewTabEnum[] = [OverviewTabEnum.NoDate, OverviewTabEnum.Expired, OverviewTabEnum.Current, OverviewTabEnum.Future]
    ) {}
}
