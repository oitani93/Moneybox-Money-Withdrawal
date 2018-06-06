using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private IAccountRepository _accountRepository;
        private INotificationService _notificationService;

        public TransferMoney(IAccountRepository accountRepository,
            INotificationService notificationService)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var fromAccount = _accountRepository.GetAccountById(fromAccountId);
            var toAccount = _accountRepository.GetAccountById(toAccountId);

            fromAccount.DebitAccount(amount);
            toAccount.CreditAccount(amount);

            _accountRepository.Update(fromAccount);
            fromAccount.SendNotifyIsFundsLowEmail(amount, _notificationService);

            _accountRepository.Update(toAccount);
            toAccount.SendNotifyIsApproachingPayInLimitEmail(amount, _notificationService);
        }
    }
}