import * as React from 'react'
import { Label, Icon } from 'semantic-ui-react'

interface IProps {
    isEditing: boolean
    onChangeEditing: () => void
    onDelete: () => void
}
export class ButtonPanel extends React.PureComponent<IProps> {

    public render() {
        const classChangeEditBtn = this.props.isEditing ? 'btn-success' : 'btn-info'
        return (
            <React.Fragment>
                <Label
                    attached='top right'
                    onClick={this.props.onDelete}
                    color='red'
                    className='recurrences-view-recurrence-item-btn btn-error'>

                    <Icon name='cancel' />
                </Label>
                <Label
                    attached='bottom right'
                    onClick={this.props.onChangeEditing}
                    className={'recurrences-view-recurrence-item-btn ' + classChangeEditBtn}>

                    { this.props.isEditing
                        ? <Icon name='checkmark' />
                        : <Icon name='pencil' />
                    }
                </Label>
            </React.Fragment>
        )
    }
}
