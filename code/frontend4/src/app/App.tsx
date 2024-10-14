import { useCallback } from 'react'
import { useAppDispatch } from '../hooks'
import { NavigationBar } from './components/NavigationBar'
import { switchToTab } from './redux/app-slice'
import { ApplicationTab } from './models/application-tab-type'
import { Overview } from '../overview/Overview'
import { RecurrentTasks } from '../recurrent/RecurrentTasks'
import { Settings } from '../settings/Settings'
import { useAppState } from './redux/app-selectors'

function App() {
    const dispatch = useAppDispatch()

    const { applicationTab } = useAppState()

    const switctToTab = useCallback(
        (applicationTab: ApplicationTab) => {
            return dispatch(switchToTab(applicationTab))
        },
        [dispatch],
    )

    return (
        <div className="container">
            {applicationTab === 'overview' && <Overview />}
            {applicationTab === 'recurrent' && <RecurrentTasks />}
            {applicationTab === 'settings' && <Settings />}

            <NavigationBar
                applicationTab={applicationTab}
                switchTo={switctToTab}
            />
        </div>
    )
}

export { App }
