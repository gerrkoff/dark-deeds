import { useCallback } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { useSignOut } from '../login/hooks/useSignOut'
import { AppInfoCard } from './components/AppInfoCard'
import { TelegramIntegrationCard } from './components/TelegramIntegrationCard'
import { UserInfoCard } from './components/UserInfoCard'
import { UserSettingsCard } from './components/UserSettingsCard'
import { startTelegram } from './redux/settings-thunk'

function Settings() {
    const dispatch = useAppDispatch()

    const { appVersion } = useAppSelector(state => state.app)
    const { user } = useAppSelector(state => state.login)
    const { startTelegramLink, isStartTelegramPending } = useAppSelector(
        state => state.settings,
    )

    const username = user?.username || ''

    const { signOut } = useSignOut()

    const startTelegramIntegration = useCallback(
        () => dispatch(startTelegram()),
        [dispatch],
    )

    return (
        <div className="row">
            <div className="col">
                <UserInfoCard username={username} signOut={signOut} />
                <UserSettingsCard
                    changeShowCompleted={() =>
                        console.log('changeShowCompleted')
                    }
                    isSaveSettingsPending={false}
                    isShowCompletedEnabled={false}
                    saveSettings={() => console.log('saveSettings')}
                />
            </div>
            <div className="col">
                <TelegramIntegrationCard
                    generateStartIntegrationLink={startTelegramIntegration}
                    isGenerateStartIntegrationLinkPending={
                        isStartTelegramPending
                    }
                    startIntegrationLink={startTelegramLink}
                />
                <AppInfoCard appVersion={appVersion} />
            </div>
        </div>
    )
}

export { Settings }
