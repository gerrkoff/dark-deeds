using System;
using Xunit;

namespace DarkDeeds.E2eTests.Attributes;

public sealed class SkipOnStagingFactAttribute : FactAttribute
{
    public SkipOnStagingFactAttribute() {
        if(IsRunningOnStaging()) {
            Skip = "Ignored on Staging";
        }
    }

    private static bool IsRunningOnStaging()
    {
        return bool.Parse(Environment.GetEnvironmentVariable("RUN_STAGING") ?? "false");
    }
}
