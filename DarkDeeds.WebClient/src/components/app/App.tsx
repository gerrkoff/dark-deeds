import * as React from 'react'
import { ToastContainer } from 'react-toastify'
import { Container, Dimmer, Loader } from 'semantic-ui-react'
import EditTaskModal from '../../containers/EditTaskModal'
import ModalConfirm from '../../containers/ModalConfirm'
import { Task } from '../../models'
import { AddTaskButton } from '../edit-task'
import { Shortcuts, Toolbar } from './'

export interface IAppProps {
    appLoading: boolean
    children: React.ReactNode
    path: string
    tasks: Task[]
    tasksSaving: boolean
    navigateTo: (path: string) => void
    loadTasks: () => void
    saveTasks: (tasks: Task[]) => void
    openEditTask: () => void
}
export class App extends React.PureComponent<IAppProps> {
    public componentDidMount() {
        this.props.loadTasks()
        setInterval(this.saveTasksIfUpdated, 5 * 1000) // TODO: should be greater
    }

    public render() {
        return (
            <React.Fragment>
                <Toolbar path={this.props.path} navigateTo={this.props.navigateTo} />
                <Container>
                    {this.props.children}
                </Container>
                <AddTaskButton openModal={this.props.openEditTask} />
                <EditTaskModal />
                <Dimmer active={this.props.appLoading}>
                    <Loader />
                </Dimmer>
                <ToastContainer />
                <Shortcuts openEditTask={this.props.openEditTask} />
                <ModalConfirm />
            </React.Fragment>
        )
    }

    private saveTasksIfUpdated = () => {
        const updated = this.props.tasks.filter(x => x.updated)

        if (updated.length === 0 || this.props.tasksSaving) {
            return
        }

        this.props.saveTasks(updated)
    }
}
