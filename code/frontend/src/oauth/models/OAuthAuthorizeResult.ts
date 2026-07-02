export type OAuthAuthorizeResult =
    | { status: 'redirect'; redirectUrl: string }
    | { status: 'needs-login' }
    | { status: 'error' }
