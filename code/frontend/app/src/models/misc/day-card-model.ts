import { Task } from 'models'

export class DayCardModel {
    constructor(public date: Date, public tasks: Task[] = []) {}
}
