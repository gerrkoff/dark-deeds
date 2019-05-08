import { Task } from '../models'
import { TaskHubApi } from '../api'
import { ToastService, UtilsService } from '../services'

export class TaskHub {

    private _reconnectingToastId: string = 'toast-reconnection-id'
    private _ready: boolean = false

    constructor(
        private reloadCallback: () => Promise<void>,
        updateCallback: (tasks: Task[], localUpdate: boolean) => void
    ) {
        TaskHubApi.hubSubscribe(
            this.reconnect,
            updateCallback,
            () => console.log('[task-hub] heartbeat'))
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
        await TaskHubApi.hubStop()
    }

    public saveTasks = (tasks: Task[]): Promise<void> => {
        return TaskHubApi.saveTasks(tasks)
    }

    private reconnect = async(): Promise<void> => {
        this._ready = false
        ToastService.info('reconnecting...', { autoClose: false, closeOnClick: false, draggable: false, toastId: this._reconnectingToastId })
        await this.connect()
        await this.reloadCallback()
        ToastService.update(this._reconnectingToastId, 'reconnecting... done', { autoClose: 1000, hideProgressBar: true })
    }

    private connect = async(): Promise<void> => {
        let attemptCount = 1
        while (true) {
            try {
                await TaskHubApi.hubStart()
                this._ready = true
                return
            // tslint:disable-next-line:no-empty
            } catch (error) {}
            await UtilsService.delay(this.evalConnectRetryDelay(++attemptCount))
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
