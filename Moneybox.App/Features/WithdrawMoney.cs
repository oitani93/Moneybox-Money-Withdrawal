﻿using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private IAccountRepository _accountRepository;
        private INotificationService _notificationService;

        public WithdrawMoney(IAccountRepository accountRepository,
            INotificationService notificationService)
        {
            _accountRepository = accountRepository;
            _notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            var from = _accountRepository.GetAccountById(fromAccountId);

            from.DebitAccount(amount);

            _accountRepository.Update(from);
            from.SendNotifyIsFundsLowEmail(amount, _notificationService);
        }
    }
}