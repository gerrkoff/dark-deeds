import * as React from 'react'
import { Container, Dimmer, Loader } from 'semantic-ui-react'
import EditTaskModal from '../../containers/EditTaskModal'
import ModalConfirm from '../../containers/ModalConfirm'
import { Task } from '../../models'
import { AddTaskButton } from '../edit-task'
import { HealthChecker } from '../../helpers'
import { Shortcuts, Toolbar, NotSavedIndicator } from './'

export interface IAppProps {
    appLoading: boolean
    children: React.ReactNode
    path: string
    tasks: Task[]
    tasksSaving: boolean
    tasksNotSaved: boolean
    navigateTo: (path: string) => void
    loadTasks: () => void
    saveTasks: (tasks: Task[]) => void
    openEditTask: () => void
    startTaskHub: () => void
    stopTaskHub: () => void
    loadSettings: () => void
}
export class App extends React.PureComponent<IAppProps> {
    private saveTaskInterval: NodeJS.Timeout
    private healCheckInterval: NodeJS.Timeout

    public componentDidMount() {
        this.props.loadTasks()
        this.props.startTaskHub()
        this.props.loadSettings()
        this.saveTaskInterval = setInterval(this.saveTasksIfUpdated, 5 * 1000)
        // this.healCheckInterval = setInterval(HealthChecker.ping, 60 * 1000)
        window.onbeforeunload = this.confirmExit
    }

    public componentWillUnmount() {
        this.props.stopTaskHub()
        clearInterval(this.saveTaskInterval)
        clearInterval(this.healCheckInterval)
        window.onbeforeunload = null
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
                <Shortcuts openEditTask={this.props.openEditTask} />
                <ModalConfirm />
                <NotSavedIndicator active={this.props.tasksNotSaved} />
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

    private confirmExit = (event: BeforeUnloadEvent): string | void => {
        if (this.props.tasksNotSaved) {
            this.saveTasksIfUpdated()
            return ''
        }
    }
}
