import { useCallback } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { overviewSelector } from '../overview/redux/overview-selectors'
import { incrementByAmount } from '../overview/redux/overview-slice'
import { addWithDelay } from '../overview/redux/overview-thunk'
import { NavigationBar } from './components/NavigationBar'
import { appSelector } from './redux/app-selectors'
import { switchToTab } from './redux/app-slice'
import { ApplicationTab } from './models/application-tab-type'
import { Overview } from '../overview/Overview'
import { RecurrentTasks } from '../recurrent/RecurrentTasks'
import { Settings } from '../settings/Settings'

function App() {
    const count = useAppSelector(state => state.overview.value)
    const count2 = useAppSelector(overviewSelector)
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
            {count}
            {count2}

            <button onClick={() => dispatch(incrementByAmount(3))}>
                Increment
            </button>

            <button onClick={() => dispatch(addWithDelay(123))}>
                Increment with delay
            </button>

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
