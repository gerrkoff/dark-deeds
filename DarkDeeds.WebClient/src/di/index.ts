import 'reflect-metadata'
import { Container } from 'inversify'
import service from './service'

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

const container = new Container({ defaultScope: 'Singleton' })

container.bind<DateService>(service.DateService).to(DateService)
container.bind<KeyConstants>(service.KeyConstants).to(KeyConstants)
container.bind<LocalSettingsService>(service.LocalSettingsService).to(LocalSettingsService)
container.bind<StorageService>(service.StorageService).to(StorageService)
container.bind<TaskConverter>(service.TaskConverter).to(TaskConverter)
container.bind<TaskMoveService>(service.TaskMoveService).to(TaskMoveService)
container.bind<TaskService>(service.TaskService).to(TaskService)
container.bind<ToastService>(service.ToastService).to(ToastService)
container.bind<UtilsService>(service.UtilsService).to(UtilsService)

container.bind<Api>(service.Api).to(Api)
container.bind<GeneralApi>(service.GeneralApi).to(GeneralApi)
container.bind<HealthCheckApi>(service.HealthCheckApi).to(HealthCheckApi)
container.bind<LoginApi>(service.LoginApi).to(LoginApi)
container.bind<SettingsApi>(service.SettingsApi).to(SettingsApi)
container.bind<TaskApi>(service.TaskApi).to(TaskApi)
container.bind<TaskHubApi>(service.TaskHubApi).to(TaskHubApi)
container.bind<TelegramIntegrationApi>(service.TelegramIntegrationApi).to(TelegramIntegrationApi)

console.log(service)
export {
    container as di,
    service,

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
