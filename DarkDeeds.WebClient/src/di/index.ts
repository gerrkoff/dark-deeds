import 'reflect-metadata'
import { Container } from 'inversify'

import { UtilsService } from './services/utils-service'
import { DateService } from './services/date-service'
import { KeyConstants } from './services/key-constants'
import { LocalSettingsService } from './services/local-settings-service'
import { StorageService } from './services/storage-service'
import { TaskConverter } from './services/task-converter'
import { TaskMoveService } from './services/task-move-service'
import { TaskService } from './services/task-service'
import { ToastService } from './services/toast-service'

import { Api } from './api/api'
import { GeneralApi } from './api/general-api'
import { HealthCheckApi } from './api/healthcheck-api'
import { LoginApi } from './api/login-api'
import { SettingsApi } from './api/settings-api'
import { TaskApi } from './api/task-api'
import { TaskHubApi } from './api/task-hub-api'
import { TelegramIntegrationApi } from './api/telegram-integration-api'

const container = new Container()

container.bind<DateService>(DateService).toSelf()
container.bind<KeyConstants>(KeyConstants).toSelf()
container.bind<LocalSettingsService>(LocalSettingsService).toSelf()
container.bind<StorageService>(StorageService).toSelf()
container.bind<TaskConverter>(TaskConverter).toSelf()
container.bind<TaskMoveService>(TaskMoveService).toSelf()
container.bind<TaskService>(TaskService).toSelf()
container.bind<ToastService>(ToastService).toSelf()
container.bind<UtilsService>(UtilsService).toSelf()

container.bind<Api>(Api).toSelf()
container.bind<GeneralApi>(GeneralApi).toSelf()
container.bind<HealthCheckApi>(HealthCheckApi).toSelf()
container.bind<LoginApi>(LoginApi).toSelf()
container.bind<SettingsApi>(SettingsApi).toSelf()
container.bind<TaskApi>(TaskApi).toSelf()
container.bind<TaskHubApi>(TaskHubApi).toSelf()
container.bind<TelegramIntegrationApi>(TelegramIntegrationApi).toSelf()

export {
    container as di,

    UtilsService,
    DateService,
    KeyConstants,
    LocalSettingsService,
    StorageService,
    TaskConverter,
    TaskMoveService,
    TaskService,
    ToastService,

    Api,
    GeneralApi,
    HealthCheckApi,
    LoginApi,
    SettingsApi,
    TaskApi,
    TaskHubApi,
    TelegramIntegrationApi
}
