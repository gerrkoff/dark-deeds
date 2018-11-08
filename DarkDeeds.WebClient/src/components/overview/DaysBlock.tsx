import * as React from 'react'
import { DayCardModel } from '../../models'
import { DayCard } from './'

import '../../styles/days-block.css'

interface IProps {
    days: DayCardModel[],
    daysInRow?: number,
    expiredDate?: Date,
    openAddTaskModalForSpecDay?: (date: Date) => void
}
export class DaysBlock extends React.PureComponent<IProps> {
    public render() {
        const blocks = []

        this.props.days.sort((x, y) => x.date > y.date ? 1 : 0)

        if (this.props.daysInRow && this.props.daysInRow > 0) {
            const rowCount = Math.ceil(this.props.days.length / this.props.daysInRow)
            for (let i = 0; i < rowCount; i++) {
                blocks.push(this.props.days.slice(i * this.props.daysInRow, (i + 1) * this.props.daysInRow))
            }
        } else {
            blocks.push(this.props.days)
        }

        if (this.props.days.length === 0) {
            return (<div />)
        }

        return (
            <div>
                {blocks.map(x =>
                    <div className='days-block' key={x[0].date.getTime()}>
                        {x.map(y =>
                            <div className='days-block-item' key={y.date.getTime()}>
                                <DayCard day={y} expiredDate={this.props.expiredDate} openAddTaskModalForSpecDay={this.props.openAddTaskModalForSpecDay} />
                            </div>
                        )}
                    </div>
                )}
            </div>
        )
    }
}
