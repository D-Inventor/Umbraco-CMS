using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;

namespace Umbraco.Cms.Infrastructure.Runtime;

internal class RuntimeModeValidationService : IRuntimeModeValidationService
{
    private readonly IOptionsMonitor<RuntimeSettings> _runtimeSettings;
    private readonly IServiceProvider _serviceProvider;

    public RuntimeModeValidationService(IOptionsMonitor<RuntimeSettings> runtimeSettings, IServiceProvider serviceProvider)
    {
        _runtimeSettings = runtimeSettings;
        _serviceProvider = serviceProvider;
    }

    public bool Validate([NotNullWhen(false)] out string? validationErrorMessage)
    {
        var runtimeMode = _runtimeSettings.CurrentValue.Mode;
        var validationMessages = new List<string>();

        // Runtime mode validators are registered transient, but this service is registered as singleton
        foreach (var runtimeModeValidator in _serviceProvider.GetServices<IRuntimeModeValidator>())
        {
            if (runtimeModeValidator.Validate(runtimeMode, out var validationMessage) == false)
            {
                validationMessages.Add(validationMessage);
            }
        }

        if (validationMessages.Count > 0)
        {
            validationErrorMessage = $"Runtime mode validation failed for {runtimeMode}:\n" + string.Join("\n", validationMessages);
            return false;
        }

        validationErrorMessage = null;
        return true;
    }
}
