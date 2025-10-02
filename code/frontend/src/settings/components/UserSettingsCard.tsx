import { IconSliders } from '../../common/icons/IconSliders'
import { Card } from '../../common/components/Card'

interface Props {
    isShowCompletedEnabled: boolean
    changeShowCompleted: (isShowCompletedEnabled: boolean) => void
    saveSettings: () => void
    isSaveSettingsPending: boolean
    isLoadSettingsPending: boolean
}

function UserSettingsCard({
    isShowCompletedEnabled,
    changeShowCompleted,
    saveSettings,
    isSaveSettingsPending,
    isLoadSettingsPending,
}: Props) {
    return (
        <>
            <Card className="mb-2 me-2">
                <div className="card-header d-flex align-items-center">
                    <IconSliders className="me-1" />
                    User Settings
                </div>
                <div className="card-body">
                    {isLoadSettingsPending ? (
                        <div className="spinner-border" role="status">
                            <span className="visually-hidden">Loading...</span>
                        </div>
                    ) : (
                        <>
                            <div className="form-check mb-3">
                                <input
                                    className="form-check-input"
                                    type="checkbox"
                                    checked={isShowCompletedEnabled}
                                    onChange={e => changeShowCompleted(e.target.checked)}
                                    id="showCompletedCheckbox"
                                    disabled={isSaveSettingsPending}
                                />
                                <label className="form-check-label" htmlFor="showCompletedCheckbox">
                                    Show completed tasks
                                </label>
                            </div>
                            <button
                                type="button"
                                className="btn btn-outline-primary"
                                onClick={saveSettings}
                                disabled={isSaveSettingsPending}
                                style={{ minWidth: '70px' }}
                            >
                                {isSaveSettingsPending ? (
                                    <>
                                        <span
                                            className="spinner-border spinner-border-sm"
                                            aria-hidden="true"
                                            data-test-id="btn-loader"
                                        ></span>
                                    </>
                                ) : (
                                    'Save'
                                )}
                            </button>
                        </>
                    )}
                </div>
            </Card>
        </>
    )
}

export { UserSettingsCard }
