import { TaskModel, TaskTimeTypeEnum } from '../models'

const service = {
    convertStringToModel(text: string): TaskModel {
        let year: number = new Date().getFullYear()
        let month: number = 0
        let day: number = 1
        let hour: number = 0
        let minute: number = 0
        let timeType: TaskTimeTypeEnum = TaskTimeTypeEnum.NoTime
        let noDate: boolean = true
        let isProbable: boolean = false

        if (/^\d{8}\s/.test(text)) {
            year = Number(text.substr(0, 4))
            month = Number(text.substr(4, 2))
            day = Number(text.substr(6, 2))
            noDate = false
            text = text.slice(9)
        } else if (/^\d{4}\s/.test(text)) {
            month = Number(text.substr(0, 2))
            day = Number(text.substr(2, 2))
            noDate = false
            text = text.slice(5)
        }

        if (/^\d{4}\s/.test(text)) {
            hour = Number(text.substr(0, 2))
            minute = Number(text.substr(2, 2))
            timeType = TaskTimeTypeEnum.ConcreteTime
            text = text.slice(5)
        } else if (/^>\d{4}\s/.test(text)) {
            hour = Number(text.substr(1, 2))
            minute = Number(text.substr(3, 2))
            timeType = TaskTimeTypeEnum.AfterTime
            text = text.slice(6)
        }

        if (/ \?$/.test(text)) {
            isProbable = true
            text = text.slice(0, text.length - 2)
        }

        return {
            dateTime: noDate ? null : new Date(year, month - 1, day, hour, minute),
            timeType,
            title: text,
            isProbable
        }
    },

    convertModelToString(model: TaskModel): string {
        let s: string = ''

        if (model.dateTime !== null) {
            if (new Date().getFullYear() !== model.dateTime.getFullYear()) {
                s += model.dateTime.getFullYear().toString()
            }
            s += `${str2digits(model.dateTime.getMonth() + 1)}${str2digits(model.dateTime.getDate())} `

            if (model.timeType !== TaskTimeTypeEnum.NoTime) {
                if (model.timeType === TaskTimeTypeEnum.AfterTime) {
                    s += '>'
                }
                s += `${str2digits(model.dateTime.getHours())}${str2digits(model.dateTime.getMinutes())} `
            }
        }

        let suffix: string = ''
        if (model.isProbable) {
            suffix = ' ?'
        }

        return `${s}${model.title}${suffix}`
    }
}

function str2digits(n: number): string {
    return n < 10 ? '0' + n : n.toString()
}

export { service as TaskConverter }
