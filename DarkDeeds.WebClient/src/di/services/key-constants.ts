import { injectable } from 'inversify'

@injectable()
export class KeyConstants {
    public readonly CMD_LEFT = 'MetaLeft'
    public readonly CMD_RIGHT = 'MetaRight'
    public readonly CMD_LEFT_FIREFOX = 'OSLeft'
    public readonly CMD_RIGHT_FIREFOX = 'OSRight'
    public readonly ENTER = 'Enter'
    public readonly N = 'KeyN'
    public readonly R = 'KeyR'
}
