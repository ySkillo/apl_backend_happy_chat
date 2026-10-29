using HappyChat.Domain.Common;

namespace HappyChat.Domain.Users.Exceptions;

public class AlreadyFollowingException : DomainException
{
   public AlreadyFollowingException() : base("Este usuário já está na sua lista de seguidos.") {}
}