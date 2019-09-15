import { TaskModel, TaskTypeEnum } from '../..'

export class Task implements TaskModel {
    constructor(
        public clientId: number,
        public title: string,
        public date: Date | null = null,
        public order: number = 0,
        public changed: boolean = false,
        public id: number = 0,
        public completed: boolean = false,
        public deleted: boolean = false,
        public type: TaskTypeEnum = TaskTypeEnum.Simple,
        public isProbable: boolean = false,
        public version: number = 0,
        public time: number | null = null
    ) {}

    // Change model - change tasksEqual method in TaskService
}
