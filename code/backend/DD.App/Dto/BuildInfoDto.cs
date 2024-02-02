using System.Reflection;

namespace DD.App.Dto;

public class BuildInfoDto
{
    public BuildInfoDto(Type app)
    {
        AppVersion = GetAssemblyVersion(app);
    }

    public string AppVersion { get; }

    private string GetAssemblyVersion(Type type)
    {
        return type.Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;
    }
}
