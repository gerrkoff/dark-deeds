import { useCallback, useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { NavigationBar } from './components/NavigationBar'
import { switchToTab } from './redux/app-slice'
import { ApplicationTabType } from './models/ApplicationTabType'
import { Overview } from '../overview/Overview'
import { Settings } from '../settings/Settings'
import { Login } from '../login/Login'
import { fetchBuildInfo } from './redux/app-thunk'
import { loadLocalSettings } from '../settings/redux/settings-slice'
import { localSettingsService } from '../settings/services/LocalSettingsService'
import { useLocalSettingsTracking } from '../settings/hooks/useLocalSettingsTracking'
import { StatusPanel } from '../status-panel/StatusPanel'
import { useTasksHub } from '../tasks/hooks/useTasksHub'
import { useCurrentUser } from '../login/hooks/useCurrentUser'
import { useTokenRenewal } from '../login/hooks/useTokenRenewal'
import { Recurrences } from '../recurrences/Recurrences'

function App() {
    const dispatch = useAppDispatch()

    const { applicationTab } = useAppSelector(state => state.app)

    const { loadCurrentUser } = useCurrentUser()

    useTokenRenewal()

    useLocalSettingsTracking()

    useTasksHub()

    useEffect(() => {
        loadCurrentUser()
        dispatch(fetchBuildInfo())
        dispatch(loadLocalSettings(localSettingsService.load()))
    }, [dispatch, loadCurrentUser])

    const switctToTab = useCallback(
        (applicationTab: ApplicationTabType) => {
            return dispatch(switchToTab(applicationTab))
        },
        [dispatch],
    )

    return (
        <div className="container-xxl pt-2" style={{ paddingBottom: '60px' }}>
            <>
                <StatusPanel />
                {applicationTab === 'login' && <Login />}
                {applicationTab === 'overview' && <Overview />}
                {applicationTab === 'recurrent' && <Recurrences />}
                {applicationTab === 'settings' && <Settings />}

                {applicationTab !== 'login' && (
                    <NavigationBar
                        applicationTab={applicationTab}
                        switchTo={switctToTab}
                    />
                )}
            </>
        </div>
    )
}

export { App }
