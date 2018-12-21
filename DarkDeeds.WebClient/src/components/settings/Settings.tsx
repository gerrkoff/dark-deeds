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
                <span>Hi, {this.props.username}!</span>
                <Button onClick={this.props.signout}>Sign out</Button>
            </Segment>
        )
    }
}
