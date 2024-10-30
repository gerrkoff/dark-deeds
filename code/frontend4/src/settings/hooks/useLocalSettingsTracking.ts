import { useEffect } from 'react'
import { useAppSelector } from '../../hooks'
import { localSettingsService } from '../services/LocalSettingsService'

export function useLocalSettingsTracking() {
    const { isLocalSettingsLoaded, overviewTabsExpanded, isDebugEnabled } =
        useAppSelector(state => state.settings)

    useEffect(() => {
        if (!isLocalSettingsLoaded) {
            return
        }
        return localSettingsService.save({
            overviewTabsExpanded,
            isDebugEnabled,
        })
    }, [isDebugEnabled, isLocalSettingsLoaded, overviewTabsExpanded])
}
