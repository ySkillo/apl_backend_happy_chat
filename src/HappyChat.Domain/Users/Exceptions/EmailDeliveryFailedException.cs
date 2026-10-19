using HappyChat.Domain.Common;

namespace HappyChat.Domain.Users.Exceptions;

public class EmailDeliveryFailedException : DomainException
{
    public EmailDeliveryFailedException()
        : base("Não foi possível enviar o e-mail de verificação. Tente novamente em instantes.") { }
}