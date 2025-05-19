using Microsoft.Identity.Client;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using System.Net.Http.Headers;


namespace PowerBIDemo;


public class PowerBIService
{
    private readonly PowerBISettings _settings;
    private readonly IHttpClientFactory _httpClientFactory;

    public PowerBIService(PowerBISettings settings, IHttpClientFactory httpClientFactory)
    {
        _settings = settings;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<object> GetEmbedInfoAsync()
    {
        string[] scopes = { "https://analysis.windows.net/powerbi/api/.default" };

        var app = ConfidentialClientApplicationBuilder.Create(_settings.ClientId)
            .WithClientSecret(_settings.ClientSecret)
            .WithAuthority(AzureCloudInstance.AzurePublic, _settings.TenantId)
            .Build();

        var authResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();

        var tokenCredentials = new TokenCredentials(authResult.AccessToken, "Bearer");

        var pbiClient = new PowerBIClient(new Uri("https://api.powerbi.com/"), tokenCredentials);

        var report = await pbiClient.Reports.GetReportInGroupAsync(
                Guid.Parse(_settings.WorkspaceId),
                Guid.Parse(_settings.ReportId)
                );

        var generateTokenRequestParameters = new GenerateTokenRequest(accessLevel: "View");
        var tokenResponse = await pbiClient.Reports.GenerateTokenAsync(
                Guid.Parse(_settings.WorkspaceId),
            Guid.Parse(_settings.ReportId),
            generateTokenRequestParameters
                );

        return new
        {
            embedToken = tokenResponse.Token,
            embedUrl = report.EmbedUrl,
            reportId = report.Id
        };
    }
}
