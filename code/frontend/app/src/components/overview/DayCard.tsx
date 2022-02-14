import * as React from 'react'
import { List, Segment } from 'semantic-ui-react'
import { DayCardModel, Task, TaskModel, TaskTypeEnum } from '../../models'
import { DayCardHeader, TaskItem } from './'

import '../../styles/day-card.css'
import { taskService } from 'src/di/services/task-service'

interface IProps {
    day: DayCardModel
    expiredDate?: Date
    openTaskModal?: (model: TaskModel, uid: string | null) => void
    changeTaskStatus?: (
        uid: string,
        completed?: boolean,
        deleted?: boolean
    ) => void
    confirmAction?: (
        content: React.ReactNode,
        action: () => void,
        header: string
    ) => void
}
export class DayCard extends React.PureComponent<IProps> {
    private taskService = taskService

    public render() {
        const className =
            this.props.expiredDate &&
            this.props.day.date < this.props.expiredDate
                ? 'day-card-expired'
                : ''
        const tasks = this.props.day.tasks.sort(this.taskService.sorting)
        return (
            <Segment id="day-card" className={className} inverted raised>
                <DayCardHeader
                    date={this.props.day.date}
                    openTaskModal={this.props.openTaskModal}
                />
                {this.renderAdditionalTaskList(
                    tasks.filter(
                        (x: Task) => x.type === TaskTypeEnum.Additional
                    )
                )}
                {this.renderTaskList(
                    tasks.filter(
                        (x: Task) => x.type !== TaskTypeEnum.Additional
                    )
                )}
            </Segment>
        )
    }

    private renderAdditionalTaskList(tasks: Task[]) {
        if (tasks.length === 0) {
            return <React.Fragment />
        }
        return (
            <List className="fixed-list">
                {tasks.map((x: Task) => (
                    <List.Item key={x.uid} className="all-day-item">
                        {this.renderTask(x)}
                    </List.Item>
                ))}
            </List>
        )
    }

    private renderTaskList(tasks: Task[]) {
        return (
            <List
                bulleted
                className="day-card-tasks-view fixed-list dragula-container"
                data-id={this.props.day.date.getTime()}
            >
                {tasks.map((x: Task) => (
                    <List.Item key={x.uid} data-id={x.uid}>
                        {this.renderTask(x)}
                    </List.Item>
                ))}
            </List>
        )
    }

    private renderTask(task: Task) {
        return (
            <TaskItem
                task={task}
                changeTaskStatus={this.props.changeTaskStatus}
                confirmAction={this.props.confirmAction}
                openTaskModal={this.props.openTaskModal}
            />
        )
    }
}
