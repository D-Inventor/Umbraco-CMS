using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Configuration.Models;

namespace Umbraco.Cms.Infrastructure.Runtime.RuntimeModeValidators;

internal class UmbracoApplicationUrlValidator : RuntimeModeProductionValidatorBase
{
    private readonly IOptionsMonitor<WebRoutingSettings> _webRoutingSettings;

    public UmbracoApplicationUrlValidator(IOptionsMonitor<WebRoutingSettings> webRoutingSettings)
        => _webRoutingSettings = webRoutingSettings;

    protected override bool Validate([NotNullWhen(false)] out string? validationErrorMessage)
    {
        // Ensure fixed Umbraco application URL is set
        if (string.IsNullOrWhiteSpace(_webRoutingSettings.CurrentValue.UmbracoApplicationUrl))
        {
            validationErrorMessage = "Umbraco application URL needs to be set in production mode.";
            return false;
        }

        validationErrorMessage = null;
        return true;
    }
}
