using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DarkDeeds.WebClientBff.Web.Exceptions;

[Serializable]
public class ModelValidationException : Exception
{
    public ICollection<ModelError> Errors { get; }

    public ModelValidationException(string message) : base("Model validation exception")
    {
        Errors = new List<ModelError> { new(message) };
    }

    public ModelValidationException(ModelStateDictionary modeState) : base("Model validation exception")
    {
        var errors = new List<ModelError>();
        foreach (ModelStateEntry modelStateEntry in modeState.Values)
        {
            errors.AddRange(modelStateEntry.Errors);
        }
        Errors = errors;
    }

    public override IDictionary Data => Errors.ToDictionary(x => x.ErrorMessage);
}
