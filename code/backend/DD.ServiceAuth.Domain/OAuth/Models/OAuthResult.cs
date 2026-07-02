using System.Diagnostics.CodeAnalysis;
using DD.ServiceAuth.Domain.OAuth.Dto;

namespace DD.ServiceAuth.Domain.OAuth.Models;

[SuppressMessage(
    "Design",
    "CA1000:Do not declare static members on generic types",
    Justification = "Static factory methods on the generic result type give call sites ergonomic OAuthResult<T>.Success/Failure without redundant type arguments.")]
public sealed class OAuthResult<T>
    where T : class
{
    private OAuthResult(T? value, OAuthErrorDto? error)
    {
        Value = value;
        Error = error;
    }

    public T? Value { get; }

    public OAuthErrorDto? Error { get; }

    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error is null;

    public static OAuthResult<T> Success(T value)
    {
        return new OAuthResult<T>(value, null);
    }

    public static OAuthResult<T> Failure(OAuthErrorDto error)
    {
        return new OAuthResult<T>(null, error);
    }
}
