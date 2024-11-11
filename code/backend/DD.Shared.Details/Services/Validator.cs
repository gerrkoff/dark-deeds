using DD.Shared.Details.Exceptions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DD.Shared.Details.Services;

public interface IValidator
{
    void Validate(ModelStateDictionary modelState);
}

internal sealed class Validator : IValidator
{
    public void Validate(ModelStateDictionary modelState)
    {
        if (!modelState.IsValid)
            throw new ModelValidationException(modelState);
    }
}
