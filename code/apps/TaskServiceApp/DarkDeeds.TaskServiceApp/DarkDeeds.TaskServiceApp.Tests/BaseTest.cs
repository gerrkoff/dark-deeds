using AutoMapper;
using DarkDeeds.TaskServiceApp.Models.Mapping;

namespace DarkDeeds.TaskServiceApp.Tests
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