import * as React from 'react'
import { Segment, Button } from 'semantic-ui-react'

interface IProps {
    username: string
    signout: () => void
}
export class Settings extends React.PureComponent<IProps> {
    public render() {
        console.log(this.props.username)
        return (
            <Segment inverted>
                <span>Hi, {this.props.username}!</span><br/><br/>
                <Button onClick={this.props.signout} size='mini'>Sign out</Button>
            </Segment>
        )
    }
}
