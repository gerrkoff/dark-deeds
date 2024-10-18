import { useEffect } from 'react'
import { useAppSelector } from '../../hooks'
import { localSettingsService } from '../services/SettingsService'

export function useLocalSettingsTracking() {
    const { isLocalSettingsLoaded, overviewTabsExpanded } = useAppSelector(
        state => state.settings,
    )

    useEffect(() => {
        if (!isLocalSettingsLoaded) {
            return
        }
        return localSettingsService.save({ overviewTabsExpanded })
    }, [isLocalSettingsLoaded, overviewTabsExpanded])
}
