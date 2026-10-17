using HappyChat.Domain.Common;

namespace HappyChat.Domain.Users.Exceptions;

public class PerfilAlreadyRegisteredException : DomainException
{
    public PerfilAlreadyRegisteredException()
        : base("Já existe um usuário com este perfil.")
    {
    }
}