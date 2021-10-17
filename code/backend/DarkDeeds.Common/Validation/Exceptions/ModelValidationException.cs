using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

namespace DarkDeeds.Common.Validation.Exceptions
{
    [Serializable]
    public class ModelValidationException : Exception
    {
        public ICollection<ValidationResult> Errors { get; }

        public ModelValidationException(string message) : base("Model validation exception")
        {
            Errors = new List<ValidationResult> { new(message) };
        }
        
        public ModelValidationException(ICollection<ValidationResult> errors) : base("Model validation exception")
        {
            Errors = errors;
        }
        
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Errors), Errors);

            base.GetObjectData(info, context);
        }

        public override IDictionary Data => Errors.ToDictionary(x => x.ErrorMessage);
    }
}