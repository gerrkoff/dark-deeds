import 'styles/day-card.css'

import { DayCardHeader, TaskItem } from 'components/overview'
import { taskService } from 'di/services/task-service'
import { DayCardModel, Task, TaskModel, TaskTypeEnum } from 'models'
import * as React from 'react'
import { List, Segment } from 'semantic-ui-react'

interface IProps {
    day: DayCardModel
    expiredDate?: Date
    routineShownDates: Set<number>
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
    toggleRoutineShown?: (date: Date) => void
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
        const includeRoutine = this.props.routineShownDates.has(this.props.day.date.getTime())
        return (
            <Segment id="day-card" className={className} inverted raised>
                <DayCardHeader
                    date={this.props.day.date}
                    isRoutineShown={includeRoutine}
                    hasRoutine={this.hasRoutine()}
                    remainingRoutineCount={this.getRemainingRoutineCount()}
                    openTaskModal={this.props.openTaskModal}
                    toggleRoutineShown={this.props.toggleRoutineShown}
                />
                {this.renderAdditionalTaskList(
                    tasks.filter(
                        (x: Task) => x.type === TaskTypeEnum.Additional
                    )
                )}
                {this.renderTaskList(
                    tasks.filter(
                        (x: Task) => x.type !== TaskTypeEnum.Additional && (includeRoutine || x.type !== TaskTypeEnum.Routine)
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
                    <List.Item
                        key={x.uid}
                        data-id={x.uid}
                        className={
                            'task-item-bullet' +
                            (x.completed ? ' completed' : '')
                        }
                    >
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

    private getRemainingRoutineCount() {
        return this.props.day.tasks.filter(
            (x: Task) => x.type === TaskTypeEnum.Routine && !x.completed
        ).length
    }

    private hasRoutine() {
        return this.props.day.tasks.some(
            (x: Task) => x.type === TaskTypeEnum.Routine
        )
    }
}
