using System.Text;

namespace HappyChat.Infrastructure.Auth.Settings;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpireMinutes { get; set; } = 60;

    /// <summary>Falha na inicialização, com mensagem clara, se a configuração estiver incompleta.</summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Key) || Encoding.UTF8.GetByteCount(Key) < 32)
            throw new InvalidOperationException(
                "Jwt:Key ausente ou curta demais (mínimo de 32 caracteres). " +
                "Em desenvolvimento: dotnet user-secrets set \"Jwt:Key\" \"<chave>\" --project src/HappyChat.API. " +
                "Em produção: variável de ambiente Jwt__Key.");

        if (string.IsNullOrWhiteSpace(Issuer))
            throw new InvalidOperationException("Jwt:Issuer não está configurado.");

        if (string.IsNullOrWhiteSpace(Audience))
            throw new InvalidOperationException("Jwt:Audience não está configurado.");

        if (ExpireMinutes <= 0)
            throw new InvalidOperationException("Jwt:ExpireMinutes deve ser maior que zero.");
    }
}
