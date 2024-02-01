﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace StudentLearningHistory.ModelBinders
{
    public class FormDataDeJsonStringBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var modelName = bindingContext.ModelName;

            // Try to fetch the value of the argument by name
            var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

            var value = valueProviderResult.FirstValue;

            // Check if the argument value is null or empty
            if (string.IsNullOrEmpty(value))
            {
                return Task.CompletedTask;
            }

            try
            {
                object result = JsonSerializer.Deserialize(value, bindingContext.ModelType);
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            catch (Exception)
            {

                bindingContext.Result = ModelBindingResult.Failed();
            }
            return Task.CompletedTask;
        }
    }
}
