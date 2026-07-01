using System.Security.Cryptography;
using System.Text;
using DD.ServiceAuth.Domain.Services;
using Xunit;

namespace DD.Tests.Unit.ServiceAuth;

public class PkceServiceTests
{
    // RFC 7636 Appendix B reference test vector.
    private const string RfcVerifier = "dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk";
    private const string RfcChallenge = "E9Melhoa2OwvFrEMTJguCHaoeK1t8URWbuGJSstw-cM";

    private readonly PkceService _service = new();

    [Fact]
    public void Verify_CorrectS256Pair_ReturnsTrue()
    {
        // Act
        var result = _service.Verify(RfcVerifier, RfcChallenge);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Verify_ComputedS256Pair_ReturnsTrue()
    {
        // Arrange
        const string verifier = "a-freshly-generated-code-verifier-value-123456";
        var challenge = ComputeChallenge(verifier);

        // Act
        var result = _service.Verify(verifier, challenge);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Verify_WrongChallenge_ReturnsFalse()
    {
        // Act
        var result = _service.Verify(RfcVerifier, "not-the-matching-challenge");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Verify_WrongVerifier_ReturnsFalse()
    {
        // Act
        var result = _service.Verify("some-other-verifier-that-does-not-match", RfcChallenge);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(null, RfcChallenge)]
    [InlineData("", RfcChallenge)]
    [InlineData(RfcVerifier, null)]
    [InlineData(RfcVerifier, "")]
    [InlineData(null, null)]
    public void Verify_MissingInput_ReturnsFalse(string? codeVerifier, string? codeChallenge)
    {
        // Act
        var result = _service.Verify(codeVerifier!, codeChallenge!);

        // Assert
        Assert.False(result);
    }

    private static string ComputeChallenge(string verifier)
    {
        var hash = SHA256.HashData(Encoding.ASCII.GetBytes(verifier));
        return Convert.ToBase64String(hash)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
