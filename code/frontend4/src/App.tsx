import './App.css'
import { DayCard } from './uikit/day-card/day-card'
import { DayCardModel } from './models/ui/day-card-model'
import { TaskTypeEnum } from './models/enums/task-type-enum'
import { Navigation } from './components/navigation/Navigation'

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
    return (
        <div className="container">
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
