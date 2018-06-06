using System;
using System.Collections.Generic;
using LeadPipe.Net.Domain;
using Moneybox.App.Domain.Services;

namespace Moneybox.App
{
    public class Account : IDomainEvent
    {
        public Account(Guid id, 
            User user,
            decimal balance,
            decimal withdrawn,
            decimal paidIn)
        {
            Id = id;
            User = user;
            Balance = balance;
            Withdrawn = withdrawn;
            PaidIn = paidIn;
            Events = new List<IDomainEvent>();
        }

        public const decimal PayInLimit = 4000m;

        public const decimal FundsLowLimit = 500m;

        public Guid Id { get; private set; }

        public User User { get; private set; }

        public decimal Balance { get; private set; }

        public decimal Withdrawn { get; private set; }

        public decimal PaidIn { get; private set; }

        public ICollection<IDomainEvent> Events { get; private set; }

        public void DebitAccount(decimal amount)
        {
            ValidateAmount(amount);
            ValidateDebitedAccount(amount);
            UpdateDebitedBalance(amount);
            UpdateWithdrawn(amount);
        }

        public void CreditAccount(decimal amount)
        {
            ValidateAmount(amount);
            ValidateCreditedAccount(amount);
            UpdateCreditedBalance(amount);
            UpdatePaidIn(amount);
        }

        public void SendNotifyIsFundsLowEmail(decimal amount, INotificationService _notificationService)
        {
            if (User != null && IsFundsLow(amount))
            {
                _notificationService.NotifyFundsLow(User.Email);
            }
        }

        public void SendNotifyIsApproachingPayInLimitEmail(decimal amount, INotificationService _notificationService)
        {
            if (User != null && IsApproachingPayInLimit(amount))
            {
                _notificationService.NotifyApproachingPayInLimit(User.Email);
            }
        }

        private void ValidateAmount(decimal amount)
        {
            if (amount <= 0m)
            {
                throw new InvalidOperationException("Amount must be greater than 0");
            }
        }

        private void ValidateDebitedAccount(decimal amount)
        {
            if (IsInsufficientFunds(amount))
            {
                throw new InvalidOperationException($"Insufficient funds for {User.Name}");
            }
        }

        private void ValidateCreditedAccount(decimal amount)
        {
            if (IsAccountPayLimitReached(amount))
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }
        }

        private bool IsInsufficientFunds(decimal amount) => (Balance - amount) < 0m;

        private bool IsFundsLow(decimal amount) => (Balance - amount) < FundsLowLimit;

        private bool IsAccountPayLimitReached(decimal amount) => (PaidIn + amount) > PayInLimit;

        private bool IsApproachingPayInLimit(decimal amount) => (PayInLimit - (PaidIn + amount)) < FundsLowLimit;

        private void UpdateDebitedBalance(decimal amount)
        {
            Balance -= amount;
            CreateEventBalanceIsZero();
        }

        private void UpdateCreditedBalance(decimal amount)
        {
            Balance += amount;
            CreateEventBalanceIsZero();
        }

        private void CreateEventBalanceIsZero()
        {
            if (Balance == 0)
            {
                Events.Add(this);
            }
        }

        private void UpdateWithdrawn(decimal amount) => Withdrawn += amount;

        private void UpdatePaidIn(decimal amount) => PaidIn += amount;
    }
}