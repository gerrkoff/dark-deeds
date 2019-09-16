import { injectable, inject } from 'inversify'
import * as signalR from '@aspnet/signalr'
import { DateService, StorageService } from '..'
import diToken from '../token'
import baseUrl from './base-url'
import { Task } from '../../models'

@injectable()
export class TaskHubApi {

    private connection: signalR.HubConnection

    public constructor(
        @inject(diToken.StorageService) private storageService: StorageService,
        @inject(diToken.DateService) private dateService: DateService
    ) {}

    public init() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(baseUrl + 'ws/task', {
                accessTokenFactory: () => this.storageService.loadAccessToken() as string
            })
            .configureLogging(signalR.LogLevel.Information)
            .build()
    }

    public hubConnected(): boolean {
        return this.connection.state === signalR.HubConnectionState.Connected
    }

    public hubStart(): Promise<void> {
        return this.connection.start()
    }

    public hubStop(): Promise<void> {
        return this.connection.stop()
    }

    public hubSubscribe(
        close: () => void,
        update: (tasks: Task[], localUpdate: boolean) => void,
        heartbeat: () => void
    ) {
        this.connection.onclose((error?: Error) => {
            if (error !== undefined) {
                close()
            }
        })
        this.connection.on('update', (tasks, localUpdate) => update(this.dateService.fixDates(tasks) as Task[], localUpdate))
        this.connection.on('heartbeat', heartbeat)
    }

    public saveTasks(tasks: Task[]): Promise<void> {
        return this.connection.send('save', tasks)
    }
}
