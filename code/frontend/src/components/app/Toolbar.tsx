import 'styles/toolbar.css'

import * as React from 'react'
import { Icon, Menu } from 'semantic-ui-react'

interface IProps {
    path: string
    navigateTo: (path: string) => void
}
export class Toolbar extends React.PureComponent<IProps> {
    public render() {
        return (
            <Menu
                widths={3}
                fluid={true}
                inverted={true}
                borderless={true}
                size="mini"
                id="app-toolbar"
            >
                {renderMenuItem(
                    this.props.navigateTo,
                    '/old',
                    'calendar',
                    this.props.path
                )}
                {renderMenuItem(
                    this.props.navigateTo,
                    '/old/recurrences',
                    'sync alternate',
                    this.props.path
                )}
                {renderMenuItem(
                    this.props.navigateTo,
                    '/old/settings',
                    'settings',
                    this.props.path
                )}
            </Menu>
        )
    }
}

function renderMenuItem(
    navigateTo: (path: string) => void,
    path: string,
    icon: any,
    currentPath: string
): React.ReactNode {
    const active =
        currentPath === '/old'
            ? path === '/old'
            : path !== '/old' && currentPath.startsWith(path)
    const dataTestId = `nav-${path}`

    return (
        <Menu.Item
            onClick={() => navigateTo(path)}
            active={active}
            data-test-id={dataTestId}
        >
            <Icon name={icon} />
        </Menu.Item>
    )
}
