using HappyChat.Application.Auth.Exceptions;
using HappyChat.Domain.Common;
using HappyChat.Domain.Users.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace HappyChat.API.Middlewares;

/// <summary>
/// Converte exceções de domínio em respostas HTTP (ProblemDetails).
/// Qualquer outra exceção segue para o tratamento padrão (500).
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            EmailAlreadyRegisteredException => (StatusCodes.Status409Conflict, "E-mail já cadastrado"),
            PerfilAlreadyRegisteredException => (StatusCodes.Status409Conflict, "Perfil já cadastrado"),
            InvalidCredentialsException => (StatusCodes.Status401Unauthorized, "Credenciais inválidas"),
            TooManyVerificationAttemptsException => (StatusCodes.Status429TooManyRequests, "Muitas tentativas"),
            EmailDeliveryFailedException => (StatusCodes.Status503ServiceUnavailable, "Falha ao enviar e-mail"),
            AlreadyFollowingException => (StatusCodes.Status409Conflict, "Você já segue este usuário"),
            UserNotFoundException => (StatusCodes.Status404NotFound, "Usuário não encontrado"),
            NotFollowingException => (StatusCodes.Status404NotFound, "Follow não encontrado"),
            DomainException => (StatusCodes.Status400BadRequest, "Regra de negócio violada"),
            _ => (0, string.Empty)
        };

        if (status == 0)
            return false;

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = exception.Message
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(
            problem, options: null, contentType: "application/problem+json", cancellationToken);

        return true;
    }
}
