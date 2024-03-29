import { TaskModel, TaskTypeEnum } from 'models'

export class Task implements TaskModel {
    constructor(
        public uid: string,
        public title: string,
        public date: Date | null = null,
        public order: number = 0,
        public changed: boolean = false,
        public completed: boolean = false,
        public deleted: boolean = false,
        public type: TaskTypeEnum = TaskTypeEnum.Simple,
        public isProbable: boolean = false,
        public version: number = 0,
        public time: number | null = null
    ) {}
}
