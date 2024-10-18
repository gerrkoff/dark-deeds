import { IconInfoLg } from '../../common/icons/IconInfoLg'
import { Card } from '../../ui/components/Card'

interface Props {
    appVersion: string
}

function AppInfoCard({ appVersion }: Props) {
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
                </div>
            </Card>
        </>
    )
}

export { AppInfoCard }
