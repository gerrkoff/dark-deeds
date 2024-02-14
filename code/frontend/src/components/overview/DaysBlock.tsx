import 'styles/days-block.css'

import { DayCard } from 'components/overview'
import { DayCardModel, TaskModel } from 'models'
import * as React from 'react'

interface IProps {
    days: DayCardModel[]
    testId?: string
    daysInRow?: number
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
export class DaysBlock extends React.PureComponent<IProps> {
    public render() {
        const blocks = []

        this.props.days.sort((x, y) => (x.date > y.date ? 1 : -1))

        if (this.props.daysInRow && this.props.daysInRow > 0) {
            const rowCount = Math.ceil(
                this.props.days.length / this.props.daysInRow
            )
            for (let i = 0; i < rowCount; i++) {
                blocks.push(
                    this.props.days.slice(
                        i * this.props.daysInRow,
                        (i + 1) * this.props.daysInRow
                    )
                )
            }
        } else {
            blocks.push(this.props.days)
        }

        if (this.props.days.length === 0) {
            return <div />
        }

        return (
            <div data-test-id={this.props.testId}>
                {blocks.map(x => (
                    <div className="days-block" key={x[0].date.getTime()}>
                        {x.map(y => (
                            <div
                                className="days-block-item"
                                key={y.date.getTime()}
                            >
                                <DayCard
                                    day={y}
                                    expiredDate={this.props.expiredDate}
                                    routineShownDates={
                                        this.props.routineShownDates
                                    }
                                    openTaskModal={this.props.openTaskModal}
                                    changeTaskStatus={
                                        this.props.changeTaskStatus
                                    }
                                    confirmAction={this.props.confirmAction}
                                    toggleRoutineShown={
                                        this.props.toggleRoutineShown
                                    }
                                />
                            </div>
                        ))}
                    </div>
                ))}
            </div>
        )
    }
}
