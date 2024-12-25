using System;

namespace Play.Identity.Service.Exceptions
{
    [Serializable]
    internal class InsufficientFundsException : Exception
    {
        public Guid UserId { get; }
        public decimal Gil { get; }

        public InsufficientFundsException(Guid userId, decimal gil)
            : base($"Not enough gil `{gil}` to debit from user {userId}")
        {
            UserId = userId;
            Gil = gil;
        }
    }
}