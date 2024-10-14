import { useCallback } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { NavigationBar } from './components/NavigationBar'
import { appSelector } from './redux/app-selectors'
import { switchToTab } from './redux/app-slice'
import { ApplicationTab } from './models/application-tab-type'
import { Overview } from '../overview/Overview'
import { RecurrentTasks } from '../recurrent/RecurrentTasks'
import { Settings } from '../settings/Settings'

function App() {
    const dispatch = useAppDispatch()

    const { applicationTab } = useAppSelector(appSelector)

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
