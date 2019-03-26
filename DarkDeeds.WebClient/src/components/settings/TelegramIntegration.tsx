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
            <table className='telegram-start-info'>
                <tbody>
                    <tr>
                        <td>
                            Start:
                        </td>
                        <td>
                            <a href={this.props.startUrl}>{this.props.startUrl}</a>
                        </td>
                        <td className='telegram-start-info-help'>
                            click to start using Dark Deeds Assistant
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Bot:
                        </td>
                        <td>
                            {this.props.botName}
                        </td>
                        <td className='telegram-start-info-help'>
                            or find him by yourself
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Command:
                        </td>
                        <td>
                            /start {this.props.chatKey}
                        </td>
                        <td className='telegram-start-info-help'>
                            and send it to him
                        </td>
                    </tr>
                </tbody>
            </table>
        </React.Fragment>
    )
}
