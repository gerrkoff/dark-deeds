import { OverviewTabEnum } from './OverviewTabEnum'

export interface SettingsLocalDto {
    overviewTabsExpanded: OverviewTabEnum[]
    isDebugEnabled: boolean
}
