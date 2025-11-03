using Interfaces;
using BankAccountSpace;
using CategorySpace;
using System.Globalization;
using Application;

namespace Facades
{
    public class AccountsFacade
    {
        private ICategoryRepository _categoryRepository;
        private IAccountRepository _accountRepository;
        public AccountsFacade(ICategoryRepository categoryRepository, IAccountRepository accountRepository)
        {
            _categoryRepository = categoryRepository;
            _accountRepository = accountRepository;
        }

        public void CreateAccount()
        {
            Console.Clear();
            System.Console.WriteLine("Давайте создадим счет!");

            var bankAccounts = _accountRepository.GetAllAccounts();
            string input;
            while (true)
            {
                System.Console.Write("\nВведите свое имя: ");
                var input_inside = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input_inside))
                {
                    System.Console.WriteLine("\nВы ничего не ввели! Попробуйте еще раз!");
                }
                else if (_accountRepository.CheckIfExistsByName(input_inside))
                {
                    System.Console.WriteLine("\nСчет с таким именем уже есть! Попробуйте еще раз!");
                }
                else if (input_inside.Trim()[0] != char.ToUpper(input_inside.Trim()[0]))
                {
                    System.Console.WriteLine("\nИмя должно быть с большой буквы! Попробуйте еще раз!");
                }
                else if (!input_inside.Any(char.IsLetter))
                {
                    System.Console.WriteLine("\nВ имени счета должна быть хотя бы одна буква! Попробуйте еще раз!");
                }
                else
                {
                    input = input_inside.Trim();
                    break;
                }
            }

            Guid account_id = Guid.NewGuid();
            BankAccount account = new BankAccount { AccountId = account_id, Name = input }; // Стартовый баланс = 0
            _accountRepository.AddNewAccount(account);

            System.Console.WriteLine("Ваш аккаунт успешно создан!\n");
            System.Console.WriteLine("Ваши данные:");
            System.Console.WriteLine($"Имя: {input}");
            System.Console.WriteLine($"Уникальный идентификатор: {account_id}");
        }

        public void DeleteOneBankAccount()
        {
            Console.Clear();
            System.Console.WriteLine("Давайте удалим выбранный вами аккаунт!\n");
            ConsoleInputHelper selector = new ConsoleInputHelper();
            var account = selector.SelectAccount(_accountRepository);

            if (account == null)
            {
                return;
            }

            while (true)
            {
                System.Console.Write("\nВы точно хотите удалить счет? (да/нет): ");
                string? cmd = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(cmd))
                {
                    System.Console.WriteLine("\nВы ничего не ввели! Попробуйте еще раз!");
                }
                else if (cmd.Trim().ToLower() == "да")
                {
                    _accountRepository.Clear(account.AccountId);
                    System.Console.WriteLine("\nВыбранный вами аккаунт был удален!");
                    return;
                }
                else if (cmd.Trim().ToLower() == "нет")
                {
                    return;
                }
                else
                {
                    System.Console.WriteLine("Введите именно да ИЛИ нет! Попробуйте еще раз!");
                }
            }
        }

        public void DeleteOperationInAccount()
        {
            Console.Clear();
            System.Console.WriteLine("Давайте удалим выбранную вами операцию из аккаунта:\n");

            ConsoleInputHelper selector = new ConsoleInputHelper();
            var account = selector.SelectAccount(_accountRepository);

            if (account == null)
            {
                return;
            }

            System.Console.WriteLine("\nВот список операций этого аккаунта:");

            var operations = account.ShowOperationsByDate(DateTime.MinValue, DateTime.MaxValue);
            for (int i = 0; i < operations.Count; i++)
            {
                System.Console.WriteLine($"\nОперация №{i + 1}");
                System.Console.WriteLine($"Дата: {operations[i].Date};");
                System.Console.WriteLine($"Категория: {_categoryRepository.FindCategoryById(operations[i].CategoryId)?.Name ?? "Категория не найдена"};");
                System.Console.WriteLine($"Сумма: {operations[i].Amount};");
                System.Console.WriteLine($"Id операции: {operations[i].OperationId};");
                System.Console.WriteLine($"Описание: {operations[i].Description}.");
            }
            ConsoleInputHelper guidReader = new ConsoleInputHelper();
            (bool success, Guid id) = guidReader.ReadGuid(account);
            if (success)
            {
                if (account.ClearOperation(id) == 1)
                {
                    System.Console.WriteLine("\nОперация была удалена!");
                    return;
                }
                System.Console.WriteLine("Операции с таким ID нет!");
            }
            else
            {
                return;
            }
        }

        public void AddSomeOperation()
        {
            Console.Clear();
            ConsoleInputHelper selector = new ConsoleInputHelper();
            BankAccount? chosen_account = selector.SelectAccount(_accountRepository);
            if (chosen_account == null)
            {
                return;
            }

            ConsoleInputHelper selectorCat = new ConsoleInputHelper();
            Category? chosen_category = selectorCat.SelectCategory(_categoryRepository);
            if (chosen_category == null)
            {
                return;
            }

            DateTime date_op;
            while (true)
            {
                System.Console.Write("\nВведите дату операции (формат dd.MM.yyyy) / (0 - назад): ");
                string? input_date = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(input_date))
                {
                    System.Console.WriteLine("\nВы ничего не ввели! Попробуйте еще раз!");
                }
                else if (input_date.Trim() == "0")
                {
                    return;
                }
                else if (!DateTime.TryParseExact(input_date.Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date_temp))
                {
                    System.Console.WriteLine("\nВы ввели дату в неверном формате! Попробуйте еще раз!");
                }
                else if (date_temp > DateTime.Now)
                {
                    System.Console.WriteLine("\nНельзя добавлять операции из будущего! Попробуйте еще раз!");
                }
                else
                {
                    date_op = date_temp;
                    break;
                }
            }

            decimal amount_op;
            while (true)
            {
                System.Console.Write("\nВведите сумму операции (строго положительная) / (0 - назад): ");
                string? input_amount = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(input_amount))
                {
                    System.Console.WriteLine("\nВы ничего не ввели! Попробуйте еще раз!");
                }
                else if (input_amount.Trim() == "0")
                {
                    return;
                }
                else if (!decimal.TryParse(input_amount.Trim(), out var amount_temp))
                {
                    System.Console.WriteLine("\nВы ввели сумму в неверном формате! Попробуйте еще раз!");
                }
                else if (amount_temp <= 0)
                {
                    System.Console.WriteLine("\nСумма операции не может быть отрицательной или равной нулю! Попробуйте еще раз!");
                }
                else
                {
                    amount_op = amount_temp;
                    break;
                }
            }

            try
            {
                System.Console.WriteLine("\nСамый последний вопрос: хотите ли вы добавить описание в операцию?");
                while (true)
                {
                    System.Console.Write("Введите (да/нет) / (0 - назад): ");
                    string? input = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(input))
                    {
                        System.Console.WriteLine("\nВы ничего не ввели! Попробуйте еще раз!");
                    }
                    else if (input.Trim() == "0")
                    {
                        return;
                    }
                    else if (input.Trim().ToLower() == "да")
                    {
                        System.Console.Write("Введите любое описание: ");
                        string? descr = Console.ReadLine() ?? "Ничего";
                        chosen_account.AddOperation(amount_op, date_op, chosen_category, descr);

                        Console.Clear();
                        System.Console.WriteLine("\nОперация была добавлена! (с описанием)");
                        System.Console.WriteLine($"\nДата операции: {date_op:dd.MM.yyyy}");
                        System.Console.WriteLine($"Категория: {chosen_category.Name}");
                        System.Console.WriteLine($"Сумма: {amount_op:F2}\n");
                        System.Console.WriteLine($"Новый баланс: {chosen_account.Balance:F2}");
                        return;
                    }
                    else if (input.Trim().ToLower() == "нет")
                    {
                        chosen_account.AddOperation(amount_op, date_op, chosen_category);

                        Console.Clear();
                        System.Console.WriteLine("\nОперация была добавлена! (без описания)");
                        System.Console.WriteLine($"\nДата операции: {date_op:dd.MM.yyyy}");
                        System.Console.WriteLine($"Категория: {chosen_category.Name}");
                        System.Console.WriteLine($"Сумма: {amount_op:F2}\n");
                        System.Console.WriteLine($"Новый баланс: {chosen_account.Balance:F2}");
                        return;
                    }
                    else
                    {
                        System.Console.WriteLine("Нужно дать четкий ответ: да или нет! Попробуйте еще раз!");
                    }
                }
            }
            catch (ArgumentException ex)
            {
                System.Console.WriteLine($"Ошибка: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                System.Console.WriteLine($"Ошибка: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Общая ошибка: {ex.Message}");
            }
        }

        public void ShowAccounts()
        {
            _accountRepository.ShowAllAccounts();
        }
    }
}