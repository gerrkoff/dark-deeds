import * as React from 'react'
import { Container } from 'semantic-ui-react'
import { Toolbar } from './'

interface IProps {
    children: React.ReactNode,
    path: string,
    navigateTo: (path: string) => void
}
export class App extends React.PureComponent<IProps> {
    public render() {
        return (
            <React.Fragment>
                <Toolbar path={this.props.path} navigateTo={this.props.navigateTo} />
                <Container>
                    {this.props.children}
                </Container>
            </React.Fragment>
        )
    }
}
