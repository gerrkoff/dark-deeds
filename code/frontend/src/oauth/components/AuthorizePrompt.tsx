import { Card } from '../../common/components/Card'

interface Props {
    clientId: string
    username: string
    error: string | null
    isSubmitting: boolean
    onAuthorize: () => void
    onDeny: () => void
}

function AuthorizePrompt({ clientId, username, error, isSubmitting, onAuthorize, onDeny }: Props) {
    return (
        <Card className="container" style={{ maxWidth: '500px' }}>
            <div className="p-3" data-test-id="oauth-authorize-prompt">
                <h5 className="mb-3">Authorize access</h5>
                <p className="mb-4">
                    <strong data-test-id="oauth-client-id">{clientId}</strong> wants to access your Dark Deeds account
                    {username && (
                        <>
                            {' '}
                            as <strong data-test-id="oauth-username">{username}</strong>
                        </>
                    )}
                    .
                </p>
                {error && (
                    <div className="alert alert-danger" role="alert">
                        {error}
                    </div>
                )}
                <div className="d-flex gap-2">
                    <button
                        type="button"
                        className="btn btn-primary"
                        style={{ minWidth: '120px' }}
                        disabled={isSubmitting}
                        onClick={onAuthorize}
                        data-test-id="btn-oauth-authorize"
                    >
                        {isSubmitting ? (
                            <span
                                className="spinner-border spinner-border-sm"
                                aria-hidden="true"
                                data-test-id="btn-loader"
                            ></span>
                        ) : (
                            'Authorize'
                        )}
                    </button>
                    <button
                        type="button"
                        className="btn btn-outline-secondary"
                        style={{ minWidth: '120px' }}
                        disabled={isSubmitting}
                        onClick={onDeny}
                        data-test-id="btn-oauth-deny"
                    >
                        Deny
                    </button>
                </div>
            </div>
        </Card>
    )
}

export { AuthorizePrompt }
