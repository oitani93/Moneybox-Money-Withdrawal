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

        public void Transfer(Account toAccount, decimal amount)
        {
            ValidateAmount(amount);
            ValidateAccount(amount);
            ValidateAccount(toAccount, amount);
            UpdateBalance(amount);
            UpdateBalance(toAccount, amount);
            UpdatePaidIn(toAccount, amount);
        }

        public void Withdraw(decimal amount)
        {
            ValidateAmount(amount);
            ValidateAccount(amount);
            UpdateBalance(amount);
            UpdateWithdrawn(amount);
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

        private void ValidateAccount(decimal amount)
        {
            if (IsInsufficientFunds(amount))
            {
                throw new InvalidOperationException($"Insufficient funds for {User.Name}");
            }
        }

        private void ValidateAccount(Account account, decimal amount)
        {
            if (account == null)
            {
                throw new InvalidOperationException("Invalid Account! Please try again");
            }
            else if (account.IsAccountPayLimitReached(amount))
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }
        }

        private bool IsInsufficientFunds(decimal amount) => (Balance - amount) < 0m;

        private bool IsFundsLow(decimal amount) => (Balance - amount) < FundsLowLimit;

        private bool IsAccountPayLimitReached(decimal amount) => (PaidIn + amount) > PayInLimit;

        private bool IsApproachingPayInLimit(decimal amount) => (PayInLimit - (PaidIn + amount)) < FundsLowLimit;

        private void UpdateBalance(decimal amount)
        {
            Balance -= amount;
            CreateEventBalanceIsZero(this);
        }

        private void UpdateBalance(Account account, decimal amount)
        {
            account.Balance += amount;
            CreateEventBalanceIsZero(account);
        }

        private void CreateEventBalanceIsZero(Account account)
        {
            if (account.Balance == 0)
            {
                Events.Add(account);
            }
        }

        private void UpdateWithdrawn(decimal amount) => Withdrawn += amount;

        private void UpdatePaidIn(Account account, decimal amount) => account.PaidIn += amount;
    }
}