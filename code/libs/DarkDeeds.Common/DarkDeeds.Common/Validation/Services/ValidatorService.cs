using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DarkDeeds.Common.Validation.Exceptions;

namespace DarkDeeds.Common.Validation.Services
{
    public class ValidatorService : IValidatorService
    {
        public void Validate<T>(T input)
        {
            var context = new ValidationContext(input, null, null);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(input, context, validationResults, true);

            if (!isValid)
            {
                throw new ModelValidationException(validationResults);
            }
        }
    }
}