namespace Moneybox.App.Domain.Services
{
    public interface IAccountService
    {
        void Withdraw(Account account, decimal amount);

        void Transfer(Account fromAccount, Account toAccount, decimal amount);
    }
}