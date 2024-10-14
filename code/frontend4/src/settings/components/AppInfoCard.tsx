import { Card } from '../../ui/components/Card'

interface Props {
    appVersion: string
}

function AppInfoCard({ appVersion }: Props) {
    return (
        <>
            <Card className="mb-2 me-2" style={{ maxWidth: '500px' }}>
                <div className="card-header">App Information</div>
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
