import { useCallback, useEffect } from 'react'
import { useAppDispatch } from '../hooks'
import { NavigationBar } from './components/NavigationBar'
import { switchToTab } from './redux/app-slice'
import { ApplicationTabType } from './models/ApplicationTabType'
import { Overview } from '../overview/Overview'
import { RecurrentTasks } from '../recurrent/RecurrentTasks'
import { Settings } from '../settings/Settings'
import { useApplicationTab } from './redux/app-selectors'
import { Login } from '../login/Login'
import { fetchBuildInfo } from './redux/app-thunk'
import { useCurrentUserLoader } from '../login/hooks/useCurrentUserLoader'
import { WelcomeState } from './WelcomeState'
import { useLoginState } from '../login/redux/login-selectors'

function App() {
    const dispatch = useAppDispatch()

    const applicationTab = useApplicationTab()

    const { loadCurrentUser } = useCurrentUserLoader()

    const { isFetchUserPending } = useLoginState()

    useEffect(() => {
        loadCurrentUser()
        dispatch(fetchBuildInfo())
    }, [dispatch, loadCurrentUser])

    const switctToTab = useCallback(
        (applicationTab: ApplicationTabType) => {
            return dispatch(switchToTab(applicationTab))
        },
        [dispatch],
    )

    return (
        <div className="container d-flex justify-content-center">
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
