import { Card } from '../../common/components/Card'
import { IconInfoLg } from '../../common/icons/IconInfoLg'

interface Props {
    appVersion: string
    isDebugEnabled: boolean
    onDebugEnabledToggle: (isEnabled: boolean) => void
}

function AppInfoCard({
    appVersion,
    isDebugEnabled,
    onDebugEnabledToggle,
}: Props) {
    return (
        <>
            <Card className="mb-2 me-2">
                <div className="card-header d-flex align-items-center">
                    <IconInfoLg className="me-1" />
                    App Information
                </div>
                <div className="card-body">
                    <p>
                        App version: <strong>{appVersion}</strong>
                    </p>
                    <div className="form-check mb-3">
                        <input
                            className="form-check-input"
                            type="checkbox"
                            checked={isDebugEnabled}
                            onChange={e =>
                                onDebugEnabledToggle(e.target.checked)
                            }
                            id="debugEnabledCheckbox"
                        />
                        <label
                            className="form-check-label"
                            htmlFor="debugEnabledCheckbox"
                        >
                            Debug mode
                        </label>
                    </div>
                </div>
            </Card>
        </>
    )
}

export { AppInfoCard }
