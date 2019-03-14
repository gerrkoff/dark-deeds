import * as React from 'react'
import { Loader } from 'semantic-ui-react'
import '../../styles/not-saved-indicator.css'

interface IProps {
    active: boolean
}
export class NotSavedIndicator extends React.PureComponent<IProps> {
    public render() {
        return (
            <div className={'not-saved-indicator'}>
                <Loader active={this.props.active} inline inverted size='tiny'/>
            </div>
        )
    }
}
