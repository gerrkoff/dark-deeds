import * as React from 'react'
import { Menu, MenuItemProps, Popup } from 'semantic-ui-react'
import '../../styles/menu-popup.css'

interface IProps {
    content: React.ReactNode
    menuItemProps: MenuItemProps[]
    changeVisibility?: (open: boolean) => void
}
interface IState {
    menuPopupOpen: boolean
}
export class MenuPopup extends React.PureComponent<IProps, IState> {
    constructor(props: IProps) {
        super(props)
        this.state = {
            menuPopupOpen: false,
        }
    }

    public render() {
        return (
            <Popup
                position="bottom left"
                on="click"
                className="menu-popup"
                open={this.state.menuPopupOpen}
                onClose={this.handleMenuPopupClose}
                onOpen={this.handleMenuPopupOpen}
                trigger={this.props.content}
                content={this.renderMenu()}
                closeOnDocumentClick
                closeOnEscape
                closeOnTriggerClick
                hideOnScroll
            />
        )
    }

    private renderMenu() {
        const adjustedProps = [...this.props.menuItemProps]

        adjustedProps.forEach((x) => {
            if (x.onClick) {
                x.onClick = this.injectClosing(x.onClick)
            }
        })

        return (
            <Menu size="mini" vertical>
                {this.props.menuItemProps.map((x) => (
                    <Menu.Item {...x} key={x.name} />
                ))}
            </Menu>
        )
    }

    private injectClosing = (func: any) => {
        return (event: any, data: any) => {
            this.handleMenuPopupClose()
            func(event, data)
        }
    }

    private handleMenuPopupClose = () => {
        if (this.props.changeVisibility) {
            this.props.changeVisibility(false)
        }
        this.setState({ menuPopupOpen: false })
    }

    private handleMenuPopupOpen = () => {
        if (this.props.changeVisibility) {
            this.props.changeVisibility(true)
        }
        this.setState({ menuPopupOpen: true })
    }
}
