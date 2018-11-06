import * as React from 'react'
import { Container, Dimmer, Loader } from 'semantic-ui-react'
import { Task } from '../../models'
import { Toolbar } from './'

interface IProps {
    appLoading: boolean,
    children: React.ReactNode,
    path: string,
    tasks: Task[],
    tasksSaving: boolean,
    navigateTo: (path: string) => void,
    loadTasks: () => void,
    saveTasks: (tasks: Task[]) => void
}
export class App extends React.PureComponent<IProps> {
    public componentDidMount() {
        this.props.loadTasks()
        setInterval(this.saveTasksIfUpdated, 15 * 1000)
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

    private saveTasksIfUpdated = () => {
        const updated = this.props.tasks.filter(x => x.updated)

        if (updated.length === 0 || this.props.tasksSaving) {
            return
        }

        console.log(`saving ${updated.length} items ${new Date()}`)

        this.props.saveTasks(updated)
    }
}
