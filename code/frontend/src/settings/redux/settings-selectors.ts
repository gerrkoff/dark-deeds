import { createSelector } from '@reduxjs/toolkit'
import { RootState } from '../../store'
import { OverviewTabEnum } from '../models/OverviewTabEnum'

export const overviewTabsExpandedSelector = createSelector(
    (state: RootState) => state.settings.overviewTabsExpanded,
    state => ({
        isNoDateExpanded: state.includes(OverviewTabEnum.NoDate),
        isExpiredExpanded: state.includes(OverviewTabEnum.Expired),
        isCurrentExpanded: state.includes(OverviewTabEnum.Current),
        isFutureExpanded: state.includes(OverviewTabEnum.Future),
    }),
)
