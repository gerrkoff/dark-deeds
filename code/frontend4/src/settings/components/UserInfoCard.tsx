import { IconPerson } from '../../common/icons/IconPerson'
import { Card } from '../../ui/components/Card'

interface Props {
    username: string
    signOut: () => void
}

function UserInfoCard({ username, signOut }: Props) {
    return (
        <>
            <Card className="mb-2 me-2">
                <div className="card-header d-flex align-items-center">
                    <IconPerson className="me-1" />
                    User Information
                </div>
                <div className="card-body">
                    <p>
                        Hello, <strong>{username}</strong>!
                    </p>
                    <button
                        className="btn btn-outline-primary"
                        onClick={signOut}
                    >
                        Sign out
                    </button>
                </div>
            </Card>
        </>
    )
}

export { UserInfoCard }
