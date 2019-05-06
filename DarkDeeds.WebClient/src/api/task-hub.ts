import * as signalR from '@aspnet/signalr'
import baseUrl from './base-url'
import { Task } from '../models'
import { DateService, StorageService } from '../services'

const connection = new signalR.HubConnectionBuilder()
    .withUrl(baseUrl + 'ws/task', {
        accessTokenFactory: () => StorageService.loadAccessToken() as string
    })
    .configureLogging(signalR.LogLevel.Information)
    .build()

const service = {
    hubStart(): Promise<void> {
        return connection.start()
            .catch(error => console.error('!!!', error.toString()))
    },

    hubStop(): Promise<void> {
        connection.off('update')
        return connection.stop()
    },

    hubSubscribe(
        update: (tasks: Task[], localUpdate: boolean) => void,
        heartbeat?: () => void
    ) {
        connection.on('update', (tasks, localUpdate) => update(DateService.fixDates(tasks) as Task[], localUpdate))

        if (heartbeat !== undefined) {
            connection.on('heartbeat', heartbeat)
        }
    },

    saveTasks(tasks: Task[]): Promise<void> {
        return connection.send('save', tasks)
    }
}

export { service as TaskHub }
