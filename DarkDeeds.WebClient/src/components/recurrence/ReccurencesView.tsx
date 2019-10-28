import * as React from 'react'
import { Button } from 'semantic-ui-react'

interface IProps {
    isCreatingRecurrences: boolean
    createRecurrences: () => void
}
export class RecurrencesView extends React.PureComponent<IProps> {
    public render() {
        return (
            <Button onClick={this.props.createRecurrences} size='mini' loading={this.props.isCreatingRecurrences}>Create Recurrences</Button>
        )
    }
}
