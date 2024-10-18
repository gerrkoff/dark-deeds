import { useMemo, useState } from 'react'
import { localSettingsService } from '../services/SettingsService'
import { OverviewTabEnum } from '../models/OverviewTabEnum'

interface Output {
    isNoDateInitExpanded: boolean
    isExpiredInitExpanded: boolean
    isCurrentInitExpanded: boolean
    isFutureInitExpanded: boolean
    handleNoDateToggle: () => void
    handleExpiredToggle: () => void
    handleCurrentToggle: () => void
    handleFutureToggle: () => void
}

export function useLocalSettings(): Output {
    const [settings, setSettings] = useState(localSettingsService.load())

    const initExpandedTabs = useMemo(
        () => ({
            isNoDateInitExpanded:
                settings.overviewOpenedTabs.indexOf(OverviewTabEnum.NoDate) >
                -1,
            isExpiredInitExpanded:
                settings.overviewOpenedTabs.indexOf(OverviewTabEnum.Expired) >
                -1,
            isCurrentInitExpanded:
                settings.overviewOpenedTabs.indexOf(OverviewTabEnum.Current) >
                -1,
            isFutureInitExpanded:
                settings.overviewOpenedTabs.indexOf(OverviewTabEnum.Future) >
                -1,
        }),
        // eslint-disable-next-line react-hooks/exhaustive-deps
        [],
    )

    const tabToggleHandlers = useMemo(() => {
        const handler = (tab: OverviewTabEnum) => {
            setSettings(oldSettings => {
                const hasTab = oldSettings.overviewOpenedTabs.indexOf(tab) > -1

                const newSettings = hasTab
                    ? {
                          ...oldSettings,
                          overviewOpenedTabs:
                              oldSettings.overviewOpenedTabs.filter(
                                  t => t !== tab,
                              ),
                      }
                    : {
                          ...oldSettings,
                          overviewOpenedTabs: [
                              ...oldSettings.overviewOpenedTabs,
                              tab,
                          ],
                      }

                localSettingsService.save(newSettings)
                return newSettings
            })
        }

        return {
            handleNoDateToggle: () => handler(OverviewTabEnum.NoDate),
            handleExpiredToggle: () => handler(OverviewTabEnum.Expired),
            handleCurrentToggle: () => handler(OverviewTabEnum.Current),
            handleFutureToggle: () => handler(OverviewTabEnum.Future),
        }
    }, [])

    return {
        ...initExpandedTabs,
        ...tabToggleHandlers,
    }
}
