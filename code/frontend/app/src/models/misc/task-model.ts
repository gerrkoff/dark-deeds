import { IDateable, TaskTypeEnum } from '..'

export class TaskModel implements IDateable {
    constructor(
        public title: string,
        public date: Date | null = null,
        public type: TaskTypeEnum = TaskTypeEnum.Simple,
        public isProbable: boolean = false,
        public time: number | null = null
    ) {}
}
