using System.Diagnostics.CodeAnalysis;
using DD.TerminalClient.Domain.Profiles;

namespace DD.TerminalClient.Domain.Abstractions;

// Read/write access to the terminal client's connection profiles: the built-in seeded profiles plus
// any user-created ones. Resolution never creates directories; saving a profile persists it and
// returns the normalized, validated value.
public interface IProfileStore
{
    IReadOnlyList<TerminalProfile> List();

    bool TryResolve(string name, [NotNullWhen(true)] out TerminalProfile? profile);

    TerminalProfile Save(string name, string baseUrl);
}
