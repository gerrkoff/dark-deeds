namespace DD.TerminalClient.Details.Api;

internal sealed record SignInResponse
{
    public string Token { get; init; } = string.Empty;

    public SignInResultCode Result { get; init; }
}
