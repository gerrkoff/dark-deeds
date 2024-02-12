﻿using System.Collections;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DD.Shared.Web;

[Serializable]
public class ModelValidationException : Exception
{
    public ModelValidationException()
    {
        Errors = new List<ModelError>();
    }

    public ModelValidationException(ModelStateDictionary modeState)
        : base("Model validation exception")
    {
        var errors = new List<ModelError>();
        foreach (var modelStateEntry in modeState.Values)
        {
            errors.AddRange(modelStateEntry.Errors);
        }

        Errors = errors;
    }

    public ModelValidationException(string message)
        : base("Model validation exception")
    {
        Errors = new List<ModelError> { new(message) };
    }

    public ModelValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
        Errors = new List<ModelError> { new(message) };
    }

    public ICollection<ModelError> Errors { get; }

    public override IDictionary Data => Errors.ToDictionary(x => x.ErrorMessage);
}
