﻿using System.Collections;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DD.Shared.Details.Exceptions;

[Serializable]
public class ModelValidationException : Exception
{
    public ModelValidationException()
    {
        Errors = [];
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
        Errors = [new(message)];
    }

    public ModelValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
        Errors = [new(message)];
    }

    public ICollection<ModelError> Errors { get; }

    public override IDictionary Data => Errors.ToDictionary(x => x.ErrorMessage);
}
