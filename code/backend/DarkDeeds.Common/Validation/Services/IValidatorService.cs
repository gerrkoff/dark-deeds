namespace DarkDeeds.Common.Validation.Services
{
    public interface IValidatorService
    {
        void Validate<T>(T input);
    }
}