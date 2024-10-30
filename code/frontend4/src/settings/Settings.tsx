import { useCallback } from 'react'
import { useAppDispatch, useAppSelector } from '../hooks'
import { useSignOut } from '../login/hooks/useSignOut'
import { AppInfoCard } from './components/AppInfoCard'
import { TelegramIntegrationCard } from './components/TelegramIntegrationCard'
import { UserInfoCard } from './components/UserInfoCard'
import { UserSettingsCard } from './components/UserSettingsCard'
import { saveSharedSettings, startTelegram } from './redux/settings-thunk'
import { changeShowCompleted, toggleDebugEnabled } from './redux/settings-slice'

function Settings() {
    const dispatch = useAppDispatch()

    const { appVersion } = useAppSelector(state => state.app)
    const { user } = useAppSelector(state => state.login)
    const {
        startTelegramLink,
        isStartTelegramPending,
        isShowCompletedEnabled,
        isSaveSharedSettingsPending,
        isLoadSharedSettingsPending,
        isDebugEnabled,
    } = useAppSelector(state => state.settings)

    const username = user?.username || ''

    const { signOut } = useSignOut()

    const handleStartTelegram = useCallback(
        () => dispatch(startTelegram()),
        [dispatch],
    )

    const handleShowCompletedChange = useCallback(
        (isEnabled: boolean) => {
            dispatch(changeShowCompleted(isEnabled))
        },
        [dispatch],
    )

    const handleSaveSettings = useCallback(
        () =>
            dispatch(
                saveSharedSettings({
                    showCompleted: isShowCompletedEnabled,
                }),
            ),
        [dispatch, isShowCompletedEnabled],
    )

    const handleToggleDebugEnabled = useCallback(
        (isEnabled: boolean) => {
            dispatch(toggleDebugEnabled(isEnabled))
        },
        [dispatch],
    )

    return (
        <div className="row">
            <div className="col">
                <UserInfoCard username={username} signOut={signOut} />
                <UserSettingsCard
                    isShowCompletedEnabled={isShowCompletedEnabled}
                    changeShowCompleted={handleShowCompletedChange}
                    saveSettings={handleSaveSettings}
                    isSaveSettingsPending={isSaveSharedSettingsPending}
                    isLoadSettingsPending={isLoadSharedSettingsPending}
                />
            </div>
            <div className="col">
                <TelegramIntegrationCard
                    generateStartIntegrationLink={handleStartTelegram}
                    isGenerateStartIntegrationLinkPending={
                        isStartTelegramPending
                    }
                    startIntegrationLink={startTelegramLink}
                />
                <AppInfoCard
                    appVersion={appVersion}
                    isDebugEnabled={isDebugEnabled}
                    onDebugEnabledToggle={handleToggleDebugEnabled}
                />
            </div>
        </div>
    )
}

export { Settings }
