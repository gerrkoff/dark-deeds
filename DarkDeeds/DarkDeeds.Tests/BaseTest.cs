using AutoMapper;
using DarkDeeds.AutoMapper;

namespace DarkDeeds.Tests
{
    public class BaseTest
    {
        static BaseTest()
        {
            Mapper.Initialize(cfg => cfg.AddProfile<MappingProfile>());
        }
    }
}