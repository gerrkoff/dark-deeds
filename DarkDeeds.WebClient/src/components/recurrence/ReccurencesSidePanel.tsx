import * as React from 'react'
import { Button } from 'semantic-ui-react'

interface IProps {
    isCreatingRecurrences: boolean
    createRecurrences: () => void
}
export class RecurrencesSidePanel extends React.PureComponent<IProps> {

    public render() {
        return (
            <React.Fragment>
                <Button onClick={this.props.createRecurrences} size='mini' loading={this.props.isCreatingRecurrences}>Create Recurrences</Button>
            </React.Fragment>
        )
    }
}
