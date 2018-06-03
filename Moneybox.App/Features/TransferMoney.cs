using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private IAccountRepository _accountRepository;
        private INotificationService _notificationService;
        private IAccountService _accountService;

        public TransferMoney(IAccountRepository accountRepository,
            INotificationService notificationService,
            IAccountService accountService)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
            _accountService = accountService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var fromAccount = _accountRepository.GetAccountById(fromAccountId);
            var toAccount = _accountRepository.GetAccountById(toAccountId);

            _accountService.Transfer(fromAccount, toAccount, amount);

            _accountRepository.Update(fromAccount);

            if (fromAccount.IsFundsLow(amount))
            {
                _notificationService.NotifyFundsLow(fromAccount.User.Email);
            }

            _accountRepository.Update(toAccount);

            if (toAccount.IsApproachingPayInLimit(amount))
            {
                _notificationService.NotifyApproachingPayInLimit(toAccount.User.Email);
            }
        }
    }
}