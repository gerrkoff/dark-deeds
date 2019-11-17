import * as React from 'react'
import { Label, Icon } from 'semantic-ui-react'

interface IProps {
    edit: () => void
    delete: () => void
}
export class ButtonPanel extends React.PureComponent<IProps> {

    public render() {
        return (
            <React.Fragment>
                <Label attached='top right'><Icon name='cancel' /></Label>
                <Label attached='bottom right'><Icon name='pencil' /></Label>
            </React.Fragment>
        )
    }
}
