namespace HappyChat.Domain.Exceptions;

public class InvalidCredentialsException : DomainException
{
    // Mensagem genérica de propósito: não revela se foi o e-mail ou a senha que falhou.
    public InvalidCredentialsException()
        : base("E-mail ou senha inválidos.")
    {
    }
}
