import { useAppSelector } from '../hooks'
import { useSignOut } from '../login/hooks/useSignOut'
import { AppInfoCard } from './components/AppInfoCard'
import { TelegramIntegrationCard } from './components/TelegramIntegrationCard'
import { UserInfoCard } from './components/UserInfoCard'
import { UserSettingsCard } from './components/UserSettingsCard'

function Settings() {
    const { appVersion } = useAppSelector(state => state.app)
    const { user } = useAppSelector(state => state.login)

    const username = user?.username || ''

    const { signOut } = useSignOut()

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
                    generateStartIntegrationLink={() =>
                        console.log('generateStartIntegrationLink')
                    }
                    isGenerateStartIntegrationLinkPending={false}
                    startIntegrationLink="https://example.com"
                />
                <AppInfoCard appVersion={appVersion} />
            </div>
        </div>
    )
}

export { Settings }
