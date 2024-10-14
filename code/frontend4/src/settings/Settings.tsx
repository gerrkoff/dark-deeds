import { AppInfoCard } from './components/AppInfoCard'
import { TelegramIntegrationCard } from './components/TelegramIntegrationCard'
import { UserInfoCard } from './components/UserInfoCard'
import { UserSettingsCard } from './components/UserSettingsCard'

function Settings() {
    const username = 'John Doe'

    const handleSignOut = () => {
        console.log('Sign out')
    }

    return (
        <div className="row">
            <div className="col">
                <UserInfoCard username={username} signOut={handleSignOut} />
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
            </div>
            <div className="col">
                <AppInfoCard appVersion="1.0.0" />
            </div>
        </div>
    )
}

export { Settings }
