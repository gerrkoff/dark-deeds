import { TaskEntity } from "../entities/task-entity";

export class DayCardModel {
    constructor(
        public date: Date,
        public tasks: TaskEntity[] = [],
    ) {}
}
