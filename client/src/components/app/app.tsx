import * as React from 'react'
import { Container } from 'semantic-ui-react'
import { Toolbar } from './'

interface IProps {
    children: React.ReactNode
}
export class App extends React.Component<IProps> {
    public render() {
        return (
            <React.Fragment>
                <Toolbar />
                <Container>
                    {this.props.children}
                </Container>
            </React.Fragment>
        )
    }
}
