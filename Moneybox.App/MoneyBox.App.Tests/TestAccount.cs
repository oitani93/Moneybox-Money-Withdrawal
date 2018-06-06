using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using NUnit.Framework;
using Moq;
using System;
using System.Linq;

namespace Moneybox.App.MoneyBox.App.Tests
{
    [TestFixture]
    public class TestAccount
    {
        private User userOne;
        private User userTwo;
        private Account fromAccount;
        private Account toAccount;
        private Mock<IAccountRepository> _accountRepository;
        private Mock<INotificationService> _notificationService;

        public void PopulateData()
        {
            userOne = new User { Id = Guid.NewGuid(), Name = "Omar Itani", Email = "o@i.com" };
            userTwo = new User { Id = Guid.NewGuid(), Name = "Jamie Mansi", Email = "j@m.com" };

            fromAccount = new Account(Guid.NewGuid(), userOne, 2000, 50, 300);
            toAccount = new Account(Guid.NewGuid(), userTwo, 2000, 0, 3800);

            _accountRepository = new Mock<IAccountRepository>();
            _notificationService = new Mock<INotificationService>();
            _accountRepository.Setup(x => x.GetAccountById(fromAccount.Id)).Returns(fromAccount);
            _accountRepository.Setup(x => x.GetAccountById(toAccount.Id)).Returns(toAccount);
        }

        [Test]
        public void TestInsufficientFunds()
        {
            PopulateData();
            var withdrawMoney = new WithdrawMoney(_accountRepository.Object, _notificationService.Object);
            var ex = Assert.Throws<InvalidOperationException>(() => withdrawMoney.Execute(fromAccount.Id, 2500));
            Assert.That(ex.Message.Equals($"Insufficient funds for {fromAccount.User.Name}"));
        }

        [Test]
        public void TestFundsIsLow()
        {
            PopulateData();
            var withdrawMoney = new WithdrawMoney(_accountRepository.Object, _notificationService.Object);
            withdrawMoney.Execute(fromAccount.Id, 1800);
            _notificationService.Verify(x => x.NotifyFundsLow(fromAccount.User.Email));
            _accountRepository.Verify(x => x.Update(fromAccount));
        }

        [Test]
        public void TestNotifyAccountPayInLimitReached()
        {
            PopulateData();
            var transferMoney = new TransferMoney(_accountRepository.Object, _notificationService.Object);
            var ex = Assert.Throws<InvalidOperationException>(() => transferMoney.Execute(fromAccount.Id, toAccount.Id, 600));
            Assert.That(ex.Message.Equals("Account pay in limit reached"));
        }

        [Test]
        public void TestIsApproachingPayInLimit()
        {
            PopulateData();
            var transferMoney = new TransferMoney(_accountRepository.Object, _notificationService.Object);
            transferMoney.Execute(fromAccount.Id, toAccount.Id, 50);
            _notificationService.Verify(x => x.NotifyApproachingPayInLimit(toAccount.User.Email));
            _accountRepository.Verify(x => x.Update(toAccount));
        }

        [Test]
        public void TestInvalidAmount()
        {
            PopulateData();
            var transferMoney = new TransferMoney(_accountRepository.Object, _notificationService.Object);
            var ex = Assert.Throws<InvalidOperationException>(() => transferMoney.Execute(fromAccount.Id, toAccount.Id, -10));
            Assert.That(ex.Message.Equals("Amount must be greater than 0"));
        }

        [Test]
        public void TestEventBalanceIsZero()
        {
            PopulateData();
            var withdrawMoney = new WithdrawMoney(_accountRepository.Object, _notificationService.Object);
            withdrawMoney.Execute(fromAccount.Id, 2000);
            var account = fromAccount.Events.OfType<Account>().SingleOrDefault();
            Assert.AreEqual(account.Balance, fromAccount.Balance);
        }
    }
}