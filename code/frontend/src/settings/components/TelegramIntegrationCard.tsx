import { IconTelegram } from '../../common/icons/IconTelegram'
import { Card } from '../../common/components/Card'

interface Props {
    startIntegrationLink: string | null
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
                <div className="card-header d-flex align-items-center">
                    <IconTelegram className="me-1" />
                    Telegram Integration
                </div>
                <div className="card-body">
                    <button
                        type="button"
                        className="btn btn-outline-primary mb-2"
                        onClick={generateStartIntegrationLink}
                        disabled={isGenerateStartIntegrationLinkPending}
                        style={{ minWidth: '120px' }}
                    >
                        {isGenerateStartIntegrationLinkPending ? (
                            <>
                                <span
                                    className="spinner-border spinner-border-sm"
                                    aria-hidden="true"
                                    data-test-id="btn-loader"
                                ></span>
                            </>
                        ) : (
                            'Generate key'
                        )}
                    </button>

                    {startIntegrationLink && (
                        <div>
                            <a href={startIntegrationLink}>{startIntegrationLink}</a>
                        </div>
                    )}
                </div>
            </Card>
        </>
    )
}

export { TelegramIntegrationCard }
