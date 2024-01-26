using System.Reflection;

namespace DD.App.Dto
{
    public class BuildInfoDto
    {
        public BuildInfoDto(Type app, Type contract)
        {
            AppVersion = GetAssemblyVersion(app);
            ContractVersion = contract == null ? string.Empty : GetAssemblyVersion(contract);
        }

        public string AppVersion { get; }
        public string ContractVersion { get; }

        private string GetAssemblyVersion(Type type)
        {
            return type.Assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
        }
    }
}
