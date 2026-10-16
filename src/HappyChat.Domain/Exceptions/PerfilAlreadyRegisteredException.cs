namespace HappyChat.Domain.Exceptions;

public class PerfilAlreadyRegisteredException : DomainException
{
    public PerfilAlreadyRegisteredException()
        : base("Já existe um usuário com este perfil.")
    {
    }
}