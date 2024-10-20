import { useCallback, useEffect } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { NavigationBar } from './components/NavigationBar'
import { switchToTab } from './redux/app-slice'
import { ApplicationTabType } from './models/ApplicationTabType'
import { Overview } from '../overview/Overview'
import { RecurrentTasks } from '../recurrent/RecurrentTasks'
import { Settings } from '../settings/Settings'
import { Login } from '../login/Login'
import { fetchBuildInfo } from './redux/app-thunk'
import { useLoadCurrentUser } from '../login/hooks/useLoadCurrentUser'
import { WelcomeState } from './WelcomeState'
import { loadLocalSettings } from '../settings/redux/settings-slice'
import { localSettingsService } from '../settings/services/SettingsService'
import { useLocalSettingsTracking } from '../settings/hooks/useLocalSettingsTracking'
import { taskApi } from '../tasks/api/TaskApi'
import { dateService } from '../common/services/DateService'

function App() {
    const dispatch = useAppDispatch()

    const { applicationTab } = useAppSelector(state => state.app)
    const { isFetchUserPending } = useAppSelector(state => state.login)

    const { loadCurrentUser } = useLoadCurrentUser()

    useEffect(() => {
        loadCurrentUser()
        dispatch(fetchBuildInfo())
        dispatch(loadLocalSettings(localSettingsService.load()))
    }, [dispatch, loadCurrentUser])

    useLocalSettingsTracking()

    const switctToTab = useCallback(
        (applicationTab: ApplicationTabType) => {
            return dispatch(switchToTab(applicationTab))
        },
        [dispatch],
    )

    return (
        <div className="container pt-2" style={{ paddingBottom: '60px' }}>
            {isFetchUserPending ? (
                <WelcomeState />
            ) : (
                <>
                    {applicationTab === 'login' && <Login />}
                    {applicationTab === 'overview' && <Overview />}
                    {applicationTab === 'recurrent' && <RecurrentTasks />}
                    {applicationTab === 'settings' && <Settings />}

                    {applicationTab !== 'login' && (
                        <NavigationBar
                            applicationTab={applicationTab}
                            switchTo={switctToTab}
                        />
                    )}
                </>
            )}
        </div>
    )
}

export { App }
