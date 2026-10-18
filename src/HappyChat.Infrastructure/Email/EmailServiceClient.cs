using System.Net.Http.Json;
using HappyChat.Application.Users.Interfaces;
using Microsoft.Extensions.Logging;

namespace HappyChat.Infrastructure.Email;

public class EmailServiceClient : IEmailNotifier
{
    private readonly HttpClient _http;
    private readonly ILogger<EmailServiceClient> _logger;
    public EmailServiceClient(HttpClient http, ILogger<EmailServiceClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task SendAccountOpeningAsync(string to, string name, CancellationToken ct = default)
    {
        var payload = new
        {
            to,
            templateType = "AccountOpening", // o email-service usa JsonStringEnumConverter
            data = new Dictionary<string, string>
            {
                ["Nome"] = name,
                ["Email"] = to
            }
        };

        try
        {
            var response = await _http.PostAsJsonAsync("api/email/send", payload, ct);
            response.EnsureSuccessStatusCode(); // o serviço responde 202 Accepted
        }
        catch (Exception ex)
        {
            // Falha no e-mail não deve derrubar o cadastro do usuário.
            _logger.LogError(ex, "Falha ao chamar o email-service para {To}", to);
        }
    }
}