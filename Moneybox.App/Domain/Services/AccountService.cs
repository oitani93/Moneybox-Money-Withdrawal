using System;

namespace Moneybox.App.Domain.Services
{
    public class AccountService : IAccountService
    {
        public void Transfer(Account fromAccount, Account toAccount, decimal amount)
        {
            ValidateAmount(amount);
            ValidateFromAccount(fromAccount, amount);
            ValidateToAccount(toAccount, amount);
            UpdateFromBalance(fromAccount, amount);
            UpdateToBalance(toAccount, amount);
            UpdatePaidIn(toAccount, amount);
        }

        public void Withdraw(Account account, decimal amount)
        {
            ValidateAmount(amount);
            ValidateFromAccount(account, amount);
            UpdateFromBalance(account, amount);
            UpdateWithdrawn(account, amount);
        }

        private void ValidateAmount(decimal amount)
        {
            if (amount <= 0m)
            {
                throw new InvalidOperationException("Amount must be greater than 0");
            }
        }

        private void ValidateFromAccount(Account account, decimal amount)
        {
            if (account.IsInsufficientFunds(amount))
            {
                throw new InvalidOperationException($"Insufficient funds for {account.User.Name}");
            }
        }

        private void ValidateToAccount(Account account, decimal amount)
        {
            if (account.IsAccountPayLimitReached(amount))
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }
        }

        private void UpdateFromBalance(Account account, decimal amount) => account.Balance -= amount;

        private void UpdateToBalance(Account account, decimal amount) => account.Balance += amount;

        private void UpdateWithdrawn(Account account, decimal amount) => account.Withdrawn += amount;

        private void UpdatePaidIn(Account account, decimal amount) => account.PaidIn += amount;
    }
}