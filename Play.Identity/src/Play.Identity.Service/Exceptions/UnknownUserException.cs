using System;

namespace Play.Identity.Service.Exceptions;

[Serializable]
internal class UnknownUserException : Exception
{
    private Guid UserId { get; }

    public UnknownUserException(Guid userId) : base($"Unknown user `{userId}`")
    {
        this.UserId = userId;
    }
}