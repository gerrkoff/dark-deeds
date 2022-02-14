import * as React from 'react'
import { Button } from 'semantic-ui-react'
import { SettingsDivider } from './'

interface IProps {
    startUrl: string
    generateKeyProcessing: boolean
    generateKey: () => void
}
export class TelegramIntegration extends React.PureComponent<IProps> {
    public render() {
        return (
            <React.Fragment>
                <SettingsDivider
                    label="Telegram Integration"
                    icon="telegram plane"
                />
                <Button
                    onClick={this.props.generateKey}
                    loading={this.props.generateKeyProcessing}
                    size="mini"
                >
                    Generate key
                </Button>
                <br />
                {this.props.startUrl === '' ? (
                    <React.Fragment />
                ) : (
                    this.renderStartInfo()
                )}
            </React.Fragment>
        )
    }

    private renderStartInfo = () => (
        <React.Fragment>
            <br />
            <a href={this.props.startUrl}>{this.props.startUrl}</a>
        </React.Fragment>
    )
}
