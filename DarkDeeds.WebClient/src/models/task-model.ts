import { IDateable, TaskTimeTypeEnum } from './'

export class TaskModel implements IDateable {
    constructor(
        public title: string,
        public dateTime: Date | null = null,
        public timeType: TaskTimeTypeEnum = TaskTimeTypeEnum.NoTime
    ) {}
}
