import { TaskTypeEnum } from '../common/models/enums/task-type-enum'
import { useAppDispatch, useAppSelector } from '../hooks'
import { Navigation } from '../navigation/Navigation'
import { overviewSelector } from '../overview/redux/overview-selectors'
import { incrementByAmount } from '../overview/redux/overview-slice'
import { addWithDelay } from '../overview/redux/overview-thunk'
import { DayCard } from '../ui/components/day-card/day-card'
import { DayCardModel } from '../ui/models/day-card-model'

const rows: DayCardModel[][] = []

for (let row = 0; row < 2; row++) {
    const dayCards: DayCardModel[] = []

    for (let i = 1; i < 8; i++) {
        dayCards.push({
            date: new Date(2021, row, i),
            tasks: [
                {
                    uid: `${row}-${i}-0`,
                    title: 'Task 1',
                    date: new Date(),
                    order: 1,
                    changed: false,
                    completed: false,
                    deleted: false,
                    type: TaskTypeEnum.Additional,
                    isProbable: false,
                    version: 1,
                    time: 1,
                },
                {
                    uid: `${row}-${i}-1`,
                    title: 'Task 2',
                    date: new Date(),
                    order: 1,
                    changed: false,
                    completed: false,
                    deleted: false,
                    type: TaskTypeEnum.Simple,
                    isProbable: false,
                    version: 1,
                    time: 1,
                },
                {
                    uid: `${row}-${i}-2`,
                    title: 'Task 3',
                    date: new Date(),
                    order: 2,
                    changed: false,
                    completed: true,
                    deleted: false,
                    type: TaskTypeEnum.Simple,
                    isProbable: false,
                    version: 1,
                    time: 1,
                },
                {
                    uid: `${row}-${i}-3`,
                    title: 'Task 4',
                    date: new Date(),
                    order: 2,
                    changed: false,
                    completed: false,
                    deleted: false,
                    type: TaskTypeEnum.Simple,
                    isProbable: true,
                    version: 1,
                    time: 1,
                },
                {
                    uid: `${row}-${i}-4`,
                    title: 'Task 5',
                    date: new Date(),
                    order: 2,
                    changed: false,
                    completed: false,
                    deleted: false,
                    type: TaskTypeEnum.Routine,
                    isProbable: false,
                    version: 1,
                    time: 1,
                },
            ],
        })
    }

    rows.push(dayCards)
}

function App() {
    const count = useAppSelector(state => state.overview.value)
    const count2 = useAppSelector(overviewSelector)
    const dispatch = useAppDispatch()

    return (
        <div className="container">
            {count}
            {count2}

            <button onClick={() => dispatch(incrementByAmount(3))}>
                Increment
            </button>

            <button onClick={() => dispatch(addWithDelay(123))}>
                Increment with delay
            </button>

            {rows.map(x => (
                <div key={x[0].date.toString()} className="row g-2 mt-2">
                    {x.map(y => (
                        <div className="col-sm">
                            <DayCard key={y.date.toString()} dayCardModel={y} />
                        </div>
                    ))}
                </div>
            ))}

            <Navigation />
        </div>
    )
}

export { App }
