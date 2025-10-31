using Interfaces;
using BankAccountSpace;

namespace Application
{
    public class DataRestoreService
    {
        private readonly IAccountRepository _accounts;
        private readonly ICategoryRepository _categories;

        public DataRestoreService(IAccountRepository accounts, ICategoryRepository categories)
        {
            _accounts = accounts;
            _categories = categories;
        }

        public void RestoreFromSnapshot(BankDataSnapshot snapshot)
        {
            _categories.Clear();
            _accounts.Clear();

            foreach (var cat in snapshot.Categories)
            {
                _categories.AddNewCategory(cat);
            }

            foreach (var accDto in snapshot.Accounts)
            {
                var account = new BankAccount
                {
                    AccountId = accDto.AccountId,
                    Name = accDto.Name,
                    Balance = 0
                };

                foreach (var opDto in accDto.Operations)
                {
                    var cat = _categories.FindCategoryById(opDto.CategoryId);
                    if (cat == null)
                    {
                        System.Console.WriteLine("\nОперация не загружена, т.к. не найдена категория!\n");
                        continue;
                    }
                    account.AddOperation(opDto.Amount, opDto.Date, cat, opDto.Description ?? "Описания нет");
                }

                account.Balance = accDto.Balance;
                _accounts.AddNewAccount(account);
            }
        }
    }
}