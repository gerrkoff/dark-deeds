import { Task } from '../models'
import { di, diToken, UtilsService, TaskHubApi } from '../di'
import { EventEmitter } from 'events'

export class TaskHub {

    private _ready: boolean = false
    private _eventEmitter: EventEmitter = new EventEmitter()
    private _reconnectEventName = 'reconnect'

    private taskHubApi = di.get<TaskHubApi>(diToken.TaskHubApi)
    private utilsService = di.get<UtilsService>(diToken.UtilsService)

    constructor(
        updateCallback: (tasks: Task[], localUpdate: boolean) => void,
        heartbeatCallback: () => void
    ) {
        this.taskHubApi.hubSubscribe(
            this.reconnect,
            updateCallback,
            () => {
                console.log('[task-hub] heartbeat')
                heartbeatCallback()
            }
        )
    }

    get ready(): boolean {
        return this._ready
    }

    public start = async(): Promise<void> => {
        this._ready = false
        await this.connect()
    }

    public stop = async(): Promise<void> => {
        this._ready = false
        await this.taskHubApi.hubStop()
    }

    public saveTasks = (tasks: Task[]): Promise<void> => {
        return this.taskHubApi.saveTasks(tasks)
    }

    public addOnReconnect(handler: (reconnecting: boolean) => void) {
        this._eventEmitter.on(this._reconnectEventName, handler)
    }

    public removeOnReconnect(handler: (reconnecting: boolean) => void) {
        this._eventEmitter.off(this._reconnectEventName, handler)
    }

    private reconnect = async(): Promise<void> => {
        this._ready = false
        this._eventEmitter.emit(this._reconnectEventName, true)
        await this.connect()
        this._eventEmitter.emit(this._reconnectEventName, false)
    }

    private connect = async(): Promise<void> => {
        let attemptCount = 1
        while (true) {
            try {
                await this.taskHubApi.hubStart()
                this._ready = true
                return
            // tslint:disable-next-line:no-empty
            } catch (error) {}
            await this.utilsService.delay(this.evalConnectRetryDelay(++attemptCount))
        }
    }

    private evalConnectRetryDelay = (attemptCount: number): number => {
        if (attemptCount === 1) {
            return 0
        } else if (attemptCount === 2) {
            return 3000
        } else {
            return 7000
        }
    }
}
