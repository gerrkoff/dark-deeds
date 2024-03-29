import { Shortcuts, Toolbar } from 'components/app'
import EditTaskModal from 'containers/EditTaskModal'
import IndicatorPanel from 'containers/IndicatorPanel'
import ModalConfirm from 'containers/ModalConfirm'
import { Task } from 'models'
import * as React from 'react'
import { Container, Dimmer, Loader } from 'semantic-ui-react'

export interface IAppProps {
    appLoading: boolean
    children: React.ReactNode
    path: string
    tasks: Task[]
    tasksSaving: boolean
    tasksChanged: boolean
    navigateTo: (path: string) => void
    loadTasks: () => void
    saveTasks: (tasks: Task[]) => void
    openEditTask: () => void
    startTaskHub: () => void
    stopTaskHub: () => void
    loadSettings: () => void
    createRecurrences: () => void
}
export class App extends React.PureComponent<IAppProps> {
    private saveTaskInterval: NodeJS.Timeout | null = null

    public componentDidMount() {
        this.props.loadTasks()
        this.props.startTaskHub()
        this.props.loadSettings()
        this.saveTaskInterval = setInterval(this.saveTasksIfChanged, 2 * 1000)
        window.onbeforeunload = this.confirmExit
    }

    public componentWillUnmount() {
        this.props.stopTaskHub()
        clearInterval(this.saveTaskInterval!)
        window.onbeforeunload = null
    }

    public render() {
        return (
            <React.Fragment>
                <Toolbar
                    path={this.props.path}
                    navigateTo={this.props.navigateTo}
                />
                <Container>{this.props.children}</Container>
                <EditTaskModal />
                <Dimmer active={this.props.appLoading}>
                    <Loader />
                </Dimmer>
                <Shortcuts
                    openEditTask={this.props.openEditTask}
                    createRecurrences={this.props.createRecurrences}
                />
                <ModalConfirm />
                <IndicatorPanel />
            </React.Fragment>
        )
    }

    private saveTasksIfChanged = () => {
        if (this.props.tasksChanged && !this.props.tasksSaving) {
            this.props.saveTasks(this.props.tasks.filter(x => x.changed))
        }
    }

    private confirmExit = (event: BeforeUnloadEvent): string | void => {
        if (this.props.tasksChanged) {
            this.saveTasksIfChanged()
            return ''
        }
    }
}
