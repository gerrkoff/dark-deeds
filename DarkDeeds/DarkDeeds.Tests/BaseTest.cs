using AutoMapper;
using DarkDeeds.Models.Mapping;

namespace DarkDeeds.Tests
{
    public class BaseTest
    {
        static BaseTest()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<ModelsMappingProfile>();
            });
        }
    }
}