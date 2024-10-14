export interface SigninResultDto {
    token: string
    result: SigninResultEnum
}

export enum SigninResultEnum {
    Unknown,
    Success,
    WrongUsernamePassword,
}
