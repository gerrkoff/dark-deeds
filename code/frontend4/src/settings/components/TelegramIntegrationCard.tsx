import { Card } from '../../ui/components/Card'

interface Props {
    startIntegrationLink: string
    generateStartIntegrationLink: () => void
    isGenerateStartIntegrationLinkPending: boolean
}

function TelegramIntegrationCard({
    startIntegrationLink,
    generateStartIntegrationLink,
    isGenerateStartIntegrationLinkPending,
}: Props) {
    return (
        <>
            <Card className="mb-2 me-2">
                <div className="card-header">Telegram Integration</div>
                <div className="card-body">
                    <button
                        type="button"
                        className="btn btn-outline-primary mb-2"
                        onClick={generateStartIntegrationLink}
                        disabled={isGenerateStartIntegrationLinkPending}
                    >
                        {isGenerateStartIntegrationLinkPending ? (
                            <>
                                <span
                                    className="spinner-border spinner-border-sm"
                                    aria-hidden="true"
                                ></span>
                            </>
                        ) : (
                            'Generate key'
                        )}
                    </button>

                    <div>
                        <a href={startIntegrationLink}>
                            {startIntegrationLink}
                        </a>
                    </div>
                </div>
            </Card>
        </>
    )
}

export { TelegramIntegrationCard }
