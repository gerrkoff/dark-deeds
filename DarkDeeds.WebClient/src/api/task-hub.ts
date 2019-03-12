import * as signalR from '@aspnet/signalr'
import baseUrl from './base-url'
import { Task } from 'src/models'
import { DateHelper, StorageHelper } from 'src/helpers'

const connection = new signalR.HubConnectionBuilder()
    .withUrl(baseUrl + 'ws/task', {
        accessTokenFactory: () => StorageHelper.Load(StorageHelper.TokenKey) as string
    })
    .build()

const service = {
    hubStart(): Promise<void> {
        return connection.start()
    },

    hubStop(): Promise<void> {
        connection.off('update')
        return connection.stop()
    },

    hubSubscribe(
        update: (tasks: Task[], localUpdate: boolean) => void
    ) {
        connection.on('update', (tasks, localUpdate) => update(DateHelper.fixDates(tasks) as Task[], localUpdate))
    },

    saveTasks(tasks: Task[]): Promise<void> {
        return connection.send('save', tasks)
    }
}

export { service as TaskHub }
