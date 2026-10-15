namespace HappyChat.Domain.Exceptions;

public class EmailAlreadyRegisteredException : DomainException
{
    public EmailAlreadyRegisteredException()
        : base("Já existe um usuário cadastrado com este e-mail.")
    {
    }
}
