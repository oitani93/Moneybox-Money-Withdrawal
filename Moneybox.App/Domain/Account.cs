using System;
using Moneybox.App.DataAccess;
namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;

        public const decimal FundsLowLimit = 500m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public bool IsInsufficientFunds(decimal amount) => (Balance - amount) < 0m;

        public bool IsFundsLow(decimal amount) => (Balance - amount) < FundsLowLimit;

        public bool IsAccountPayLimitReached(decimal amount) => (PaidIn + amount) > PayInLimit;

        public bool IsApproachingPayInLimit(decimal amount) => (PayInLimit - (PaidIn + amount)) < FundsLowLimit;
    }
}