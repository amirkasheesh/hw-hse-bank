using Interfaces;
using BankAccountSpace;

namespace Infrastructure
{
    public class InMemoryAccountRepository : IAccountRepository
    {
        private List<BankAccount> _bankAccounts = new List<BankAccount>();

        public void AddNewAccount(BankAccount? account)
        {
            if (account == null)
            {
                throw new ArgumentNullException("Вместо счета был передан null-обьект!");
            }
            _bankAccounts.Add(account);
        }

        public bool CheckIfExistsByName(string? name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("Вместо строки был передан null-обьект!");
            }
            return _bankAccounts.Select(account => account.Name.ToLower()).Contains(name.Trim().ToLower());
        }

        public BankAccount? FindAccountByID(Guid id) // Можно не парсить айди, так как делаем это в Program.cs
        {
            foreach (var el in _bankAccounts)
            {
                if (el.AccountId == id)
                {
                    return el;
                }
            }
            return null;
        }

        public List<BankAccount> GetAllAccounts()

        {
            return _bankAccounts.ToList();
        }
        public void Clear() => _bankAccounts.Clear();

        public void Clear(Guid id)
        {
            for (int i = 0; i < _bankAccounts.Count; ++i)
            {
                if (_bankAccounts[i].AccountId == id)
                {
                    _bankAccounts.RemoveAt(i);
                    return;
                }
            }
            return;
        }
        public bool CheckIfExistsCategoryById(Guid id)
        {
            foreach (var el in _bankAccounts)
            {
                if (el.ShowOperations().Find(op => op.CategoryId == id) != null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}