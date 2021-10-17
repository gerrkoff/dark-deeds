using System;
using System.Reflection;

namespace DarkDeeds.Common.Misc
{
    public class BuildInfo
    {
        public BuildInfo(Type app, Type contract)
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