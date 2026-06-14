using Xunit;

namespace DarkDeeds.E2eTests.Attributes;

public sealed class ProductionBuildFactAttribute : FactAttribute
{
    public ProductionBuildFactAttribute()
    {
        if (!ProductionBuildTestsEnabled())
        {
            Skip = "Requires PROD_BUILD_TESTS=true";
        }
    }

    private static bool ProductionBuildTestsEnabled()
    {
        return bool.Parse(Environment.GetEnvironmentVariable("PROD_BUILD_TESTS") ?? "false");
    }
}
