/*
import * as React from 'react'
import { Button, Popup } from 'semantic-ui-react'
import { DateHelper } from '../../helpers'
import '../../styles/day-card-header.css'

interface IProps {
    date: Date,
    openAddTaskModalForSpecDay?: (date: Date) => void
    mouseOver?: (isOver: boolean) => void
}
export class DayCardHeader extends React.PureComponent<IProps> {
    public render() {
        const addDisabled = !this.props.openAddTaskModalForSpecDay
        return (
            <React.Fragment>
                <Popup
                    inverted
                    position='bottom left'
                    on='click'
                    trigger={this.renderContent()}
                    content={
                        <React.Fragment>
                            <Button basic inverted color='green' content='Add' onClick={this.handleAdd} disabled={addDisabled}/>
                            <Button basic inverted color='green' content='View' />
                        </React.Fragment>
                    }
                />
            </React.Fragment>
        )
    }

    private renderContent = () => {
        const mouseOverEnabled = this.props.mouseOver
        return (
            <span className='day-card-header'
                onPointerEnter={mouseOverEnabled ? this.handleMouseEnter : undefined}
                onPointerLeave={mouseOverEnabled ? this.handleMouseLeave : undefined}
            >
                {DateHelper.toLabel(this.props.date)}
            </span>
        )
    }

    private handleMouseEnter = () => {
        if (this.props.mouseOver) {
            this.props.mouseOver(true)
        }
    }

    private handleMouseLeave = () => {
        if (this.props.mouseOver) {
            this.props.mouseOver(false)
        }
    }

    private handleAdd = () => {
        if (this.props.openAddTaskModalForSpecDay) {
            this.props.openAddTaskModalForSpecDay(this.props.date)
        }
    }
}
*/
