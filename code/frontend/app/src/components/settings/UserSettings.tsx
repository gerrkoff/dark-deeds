import { SettingsDivider } from 'components/settings'
import { AppearanceThemeEnum, SettingsClient, SettingsServer } from 'models'
import * as React from 'react'
import { ISettings } from 'redux/types'
import { Button, Checkbox, Form, Radio } from 'semantic-ui-react'

interface IProps {
    settings: ISettings
    saveServerSettings: (settings: SettingsServer) => void
    changeServerSettings: (settings: SettingsServer) => void
    changeClientSettings: (settings: SettingsClient) => void
}
export class UserSettings extends React.PureComponent<IProps> {
    public render() {
        return (
            <React.Fragment>
                <SettingsDivider label="User Settings" icon="options" />
                <Form>
                    <Form.Field>
                        <Checkbox
                            label="Show completed"
                            checked={this.props.settings.showCompleted}
                            disabled={this.props.settings.loadProcessing}
                            onChange={() =>
                                this.props.changeServerSettings({
                                    ...this.settingsServer(),
                                    showCompleted:
                                        !this.props.settings.showCompleted,
                                })
                            }
                        />
                    </Form.Field>
                    <Form.Field>
                        <Button
                            onClick={this.handleSaveClick}
                            loading={this.props.settings.saveProcessing}
                            disabled={this.props.settings.loadProcessing}
                            size="mini"
                        >
                            Save
                        </Button>
                    </Form.Field>
                    <Form.Group inline>
                        <label>Appearance:</label>
                        <Form.Field>
                            <Radio
                                label="Dark"
                                name="appearanceRadioGroup"
                                checked={
                                    this.props.settings.appearanceTheme ===
                                    AppearanceThemeEnum.Dark
                                }
                                onChange={() =>
                                    this.props.changeClientSettings({
                                        ...this.settingsClient(),
                                        appearanceTheme:
                                            AppearanceThemeEnum.Dark,
                                    })
                                }
                            />
                        </Form.Field>
                        <Form.Field>
                            <Radio
                                label="Light"
                                name="appearanceRadioGroup"
                                checked={
                                    this.props.settings.appearanceTheme ===
                                    AppearanceThemeEnum.Light
                                }
                                onChange={() =>
                                    this.props.changeClientSettings({
                                        ...this.settingsClient(),
                                        appearanceTheme:
                                            AppearanceThemeEnum.Light,
                                    })
                                }
                            />
                        </Form.Field>
                        <Form.Field>
                            <Radio
                                label="VK"
                                name="appearanceRadioGroup"
                                checked={
                                    this.props.settings.appearanceTheme ===
                                    AppearanceThemeEnum.Vk
                                }
                                onChange={() =>
                                    this.props.changeClientSettings({
                                        ...this.settingsClient(),
                                        appearanceTheme: AppearanceThemeEnum.Vk,
                                    })
                                }
                            />
                        </Form.Field>
                        <Form.Field>
                            <Radio
                                label="Dolls"
                                name="appearanceRadioGroup"
                                checked={
                                    this.props.settings.appearanceTheme ===
                                    AppearanceThemeEnum.Dolls
                                }
                                onChange={() =>
                                    this.props.changeClientSettings({
                                        ...this.settingsClient(),
                                        appearanceTheme:
                                            AppearanceThemeEnum.Dolls,
                                    })
                                }
                            />
                        </Form.Field>
                        <Form.Field>
                            <Radio
                                label="Nature"
                                name="appearanceRadioGroup"
                                checked={
                                    this.props.settings.appearanceTheme ===
                                    AppearanceThemeEnum.Nature
                                }
                                onChange={() =>
                                    this.props.changeClientSettings({
                                        ...this.settingsClient(),
                                        appearanceTheme:
                                            AppearanceThemeEnum.Nature,
                                    })
                                }
                            />
                        </Form.Field>
                    </Form.Group>
                </Form>
            </React.Fragment>
        )
    }

    private settingsServer = () => {
        return {
            showCompleted: this.props.settings.showCompleted,
        }
    }

    private settingsClient = () => {
        return {
            appearanceTheme: this.props.settings.appearanceTheme,
        }
    }

    private handleSaveClick = () =>
        this.props.saveServerSettings(this.settingsServer())
}
