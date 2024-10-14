import { Card } from '../../ui/components/Card'

function UserInformationCard() {
    return (
        <>
            <Card className="mb-2 me-2" style={{ minWidth: '250px' }}>
                <div className="card-header">User Information</div>
                <div className="card-body">
                    <p>
                        Hello, <strong>John Doe</strong>!
                    </p>
                    <button className="btn btn-primary">Sign out</button>
                </div>
            </Card>
        </>
    )
}

export { UserInformationCard }
