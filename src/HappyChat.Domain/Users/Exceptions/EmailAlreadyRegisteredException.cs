using HappyChat.Domain.Common;

namespace HappyChat.Domain.Users.Exceptions;

public class EmailAlreadyRegisteredException : DomainException
{
    public EmailAlreadyRegisteredException()
        : base("Já existe um usuário cadastrado com este e-mail.")
    {
    }
}
