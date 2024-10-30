export interface SignupResultDto {
    token: string
    result: SignupResultEnum
}

export enum SignupResultEnum {
    Unknown,
    Success,
    UsernameAlreadyExists,
    PasswordInsecure,
    InvalidUsername,
}
