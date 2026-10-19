using System.Net.Http.Json;
using HappyChat.Application.Users.Interfaces;
using HappyChat.Domain.Users.Exceptions;
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
            templateType = "AccountOpening",
            data = new Dictionary<string, string>
            {
                ["Nome"] = name,
                ["Email"] = to
            }
        };

        try
        {
            var response = await _http.PostAsJsonAsync("api/email/send", payload, ct);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao chamar o email-service para {To}", to);
        }
    }
    
    public async Task SendVerificationCodeAsync(string to, string name, string code, CancellationToken ct = default)
    {
        var payload = new
        {
            to,
            templateType = "VerificationCode",
            data = new Dictionary<string, string>
            {
                ["Nome"] = name,
                ["Codigo"] = code
            }
        };

        try
        {
            var response = await _http.PostAsJsonAsync("api/email/send", payload, ct);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao enviar código de verificação para {To}", to);
            throw new EmailDeliveryFailedException();
        }
    }
}