import { DayCardModel, Task } from 'models'

export class OverviewModel {
    constructor(
        public noDate: Task[] = [],
        public expired: DayCardModel[] = [],
        public current: DayCardModel[] = [],
        public future: DayCardModel[] = []
    ) {}
}
