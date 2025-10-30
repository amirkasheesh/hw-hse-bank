using BankAccountSpace;

namespace Interfaces
{
    public interface IAccountRepository
    {
        public List<BankAccount> GetAllAccounts();
        public BankAccount? FindAccountByID(Guid id);
        public void AddNewAccount(BankAccount? account);
        public bool CheckIfExistsByName(string? name);
    }
}