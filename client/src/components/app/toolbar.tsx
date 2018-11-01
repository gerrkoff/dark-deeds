import * as React from 'react'
import { Icon, Menu } from 'semantic-ui-react'
/*
type Props = {
    history: RouterHistory
}
*/
export class Toolbar extends React.Component {
    public render() {
        return (
            <Menu widths={4} fluid={true} inverted={true} borderless={true} size='mini'>
                <Menu.Item onClick={() => alert('/')}>
                    <Icon name='calendar' />
                </Menu.Item>
                <Menu.Item onClick={() => alert('/day/19900319')}>
                    <Icon name='tasks' />
                </Menu.Item>
                <Menu.Item onClick={() => alert('/new')}>
                    <Icon name='add' />
                </Menu.Item>
                <Menu.Item onClick={() => alert('/settings')}>
                    <Icon name='settings' />
                </Menu.Item>
            </Menu>
        )
    }
}
