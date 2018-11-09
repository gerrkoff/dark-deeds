import * as React from 'react'
import { Button, Popup } from 'semantic-ui-react'
import { DateHelper } from '../../helpers'
import '../../styles/day-card-header.css'

interface IProps {
    date: Date,
    openAddTaskModalForSpecDay?: (date: Date) => void
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
                    trigger={<span className='day-card-header'>{DateHelper.toLabel(this.props.date)}</span>}
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

    private handleAdd = () => {
        if (this.props.openAddTaskModalForSpecDay) {
            this.props.openAddTaskModalForSpecDay(this.props.date)
        }
    }
}
