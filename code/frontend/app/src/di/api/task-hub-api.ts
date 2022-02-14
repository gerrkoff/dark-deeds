import * as signalR from '@microsoft/signalr'
import baseUrl from 'di/api/base-url'
import { DateService, dateService } from 'di/services/date-service'
import { StorageService, storageService } from 'di/services/storage-service'
import { Task } from 'models'

export class TaskHubApi {
    private connection: signalR.HubConnection | null = null

    public constructor(
        private storageService: StorageService,
        private dateService: DateService
    ) {}

    public init() {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl(baseUrl + 'ws/web/task', {
                accessTokenFactory: () =>
                    this.storageService.loadAccessToken() as string,
            })
            // TODO: different levels in prod
            .configureLogging(signalR.LogLevel.Information)
            .build()
    }

    public hubConnected(): boolean {
        return this.connection!.state === signalR.HubConnectionState.Connected
    }

    public hubStart(): Promise<void> {
        return this.connection!.start()
    }

    public hubStop(): Promise<void> {
        return this.connection!.stop()
    }

    public hubSubscribe(
        close: () => void,
        update: (tasks: Task[], localUpdate: boolean) => void,
        heartbeat: () => void
    ) {
        this.connection!.onclose((error?: Error) => {
            if (error !== undefined) {
                console.warn('Hub Connection was closed with error', error)
            }
            close()
        })
        this.connection!.on('update', (tasks, localUpdate) =>
            update(
                this.dateService.adjustDatesAfterLoading(tasks) as Task[],
                localUpdate
            )
        )
        this.connection!.on('heartbeat', heartbeat)
    }

    // TODO: remove
    public saveTasks(tasks: Task[]): Promise<void> {
        const fixedTasks = this.dateService.adjustDatesBeforeSaving(tasks)
        return this.connection!.send('save', fixedTasks)
    }
}

export const taskHubApi = new TaskHubApi(storageService, dateService)
