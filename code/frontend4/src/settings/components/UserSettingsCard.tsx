import { Card } from '../../ui/components/Card'

interface Props {
    isShowCompletedEnabled: boolean
    changeShowCompleted: (isShowCompletedEnabled: boolean) => void
    saveSettings: () => void
    isSaveSettingsPending: boolean
}

function UserSettingsCard({
    isShowCompletedEnabled,
    changeShowCompleted,
    saveSettings,
    isSaveSettingsPending,
}: Props) {
    return (
        <>
            <Card className="mb-2 me-2">
                <div className="card-header">User Settings</div>
                <div className="card-body">
                    <div className="form-check mb-3">
                        <input
                            className="form-check-input"
                            type="checkbox"
                            value={isShowCompletedEnabled ? 'on' : ''}
                            onChange={e =>
                                changeShowCompleted(e.target.checked)
                            }
                            id="showCompletedCheckbox"
                        />
                        <label
                            className="form-check-label"
                            htmlFor="showCompletedCheckbox"
                        >
                            Show completed tasks
                        </label>
                    </div>
                    <button
                        type="button"
                        className="btn btn-outline-primary"
                        onClick={saveSettings}
                        disabled={isSaveSettingsPending}
                    >
                        {isSaveSettingsPending ? (
                            <>
                                <span
                                    className="spinner-border spinner-border-sm"
                                    aria-hidden="true"
                                ></span>
                            </>
                        ) : (
                            'Save'
                        )}
                    </button>
                </div>
            </Card>
        </>
    )
}

export { UserSettingsCard }
