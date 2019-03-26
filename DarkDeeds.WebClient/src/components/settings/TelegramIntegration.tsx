import * as React from 'react'
import { Button } from 'semantic-ui-react'
import { SettingsDivider } from './'

interface IProps {
    chatKey: string,
    botName: string,
    startUrl: string,
    generateKey: () => void
}
export class TelegramIntegration extends React.PureComponent<IProps> {
    public render() {
        return (
            <React.Fragment>
                <SettingsDivider label='Telegram Integration' icon='user outline' />
                <Button onClick={this.props.generateKey} size='mini'>Generate key</Button><br/>
                {this.props.chatKey === ''
                    ? <React.Fragment />
                    : this.renderStartInfo()
                }
            </React.Fragment>
        )
    }

    private renderStartInfo = () => (
        <React.Fragment>
            <br />
            <a href={this.props.startUrl}>{this.props.startUrl}</a><br /><br />
            <span>or run youself at {this.props.botName}</span><br />
            <span>/start {this.props.chatKey}</span>
        </React.Fragment>
    )
}
