using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private IAccountRepository _accountRepository;
        private INotificationService _notificationService;
        private IAccountService _accountService;

        public WithdrawMoney(IAccountRepository accountRepository,
            INotificationService notificationService,
            IAccountService accountService)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
            _accountService = accountService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            var from = _accountRepository.GetAccountById(fromAccountId);

            _accountService.Withdraw(from, amount);

            _accountRepository.Update(from);

            if (from.IsFundsLow(amount))
            {
                _notificationService.NotifyFundsLow(from.User.Email);
            }
        }
    }
}