using System.Reflection;

namespace DD.App.Dto;

public class BuildInfoDto(Type app)
{
    public string AppVersion { get; } = GetAssemblyVersion(app);

    private static string GetAssemblyVersion(Type type)
    {
        return type.Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "unknown";
    }
}
