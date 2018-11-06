import * as React from 'react'
import { Container, Dimmer, Loader } from 'semantic-ui-react'
import { Task } from '../../models'
import { Toolbar } from './'

interface IProps {
    appLoading: boolean,
    children: React.ReactNode,
    path: string,
    tasks: Task[],
    navigateTo: (path: string) => void,
    loadTasks: () => void,
    saveTasks: (tasks: Task[]) => void
}
export class App extends React.PureComponent<IProps> {
    public componentDidMount() {
        this.props.loadTasks()
    }

    public render() {
        return (
            <React.Fragment>
                <Toolbar path={this.props.path} navigateTo={this.props.navigateTo} />
                <Container>
                    {this.props.children}
                </Container>
                <Dimmer active={this.props.appLoading}>
                    <Loader />
                </Dimmer>
            </React.Fragment>
        )
    }
}
