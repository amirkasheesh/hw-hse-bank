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

        public BankAccount? FindAccountByID(Guid id)
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

        public void ShowAllAccounts()
        {
            Console.Clear();
            if (_bankAccounts.Count == 0)
            {
                System.Console.WriteLine("Счетов еще нет! Создайте хотя бы один!");
                return;
            }
            System.Console.WriteLine("\tВаши доступные счета: \n");
            int res = 0;
            for (int i = 0; i < _bankAccounts.Count; ++i)
            {
                System.Console.WriteLine($"Счет №{i + 1}:");
                System.Console.WriteLine($"Имя: {_bankAccounts[i].Name}");
                System.Console.WriteLine($"Уникальный идентификатор: {_bankAccounts[i].AccountId}");
                System.Console.WriteLine($"Баланс: {_bankAccounts[i].Balance:F2}");
                System.Console.WriteLine();
                ++res;
            }
            System.Console.WriteLine($"Количество счетов: {res}\n");
        }
    }
}