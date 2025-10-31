using System.Globalization;
using Interfaces;
using BankAccountSpace;
using CategorySpace;

namespace Application
{
    public class ConsoleInputHelper
    {
        public void ShowMyAccounts(IAccountRepository accountRepo)
        {
            var bankAccounts = accountRepo.GetAllAccounts();

            if (bankAccounts.Count == 0)
            {
                System.Console.WriteLine("Счетов еще нет! Создайте хотя бы один!");
                return;
            }
            System.Console.WriteLine("\tВаши доступные счета: \n");
            int res = 0;
            for (int i = 0; i < bankAccounts.Count; ++i)
            {
                System.Console.WriteLine($"Счет №{i + 1}:");
                System.Console.WriteLine($"Имя: {bankAccounts[i].Name}");
                System.Console.WriteLine($"Уникальный идентификатор: {bankAccounts[i].AccountId}");
                System.Console.WriteLine($"Баланс: {bankAccounts[i].Balance:F2}");
                System.Console.WriteLine();
                ++res;
            }
            System.Console.WriteLine($"Количество счетов: {res}\n");
        }
        public (bool success, DateTime from, DateTime to) ReadPeriod()
        {
            DateTime data_start;
            while (true)
            {
                System.Console.Write("\nВведите дату начала (dd.MM.yyyy) / (0 - назад): ");
                string? input_start = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input_start))
                {
                    System.Console.WriteLine("\nВы ввели пустую строку! Попробуйте еще раз!");
                }
                else if (input_start.Trim() == "0")
                {
                    return (false, DateTime.MinValue, DateTime.MaxValue);
                }
                else if (!DateTime.TryParseExact(input_start.Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data_start_temp))
                {
                    System.Console.WriteLine("\nВы ввели дату в неправильном формате!");
                }
                else
                {
                    data_start = data_start_temp;
                    break;
                }

            }

            DateTime data_end;
            while (true)
            {
                System.Console.Write("\nВведите дату конца (будет включительно) / (dd.MM.yyyy) / (0 - назад): ");
                string? input_end = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input_end))
                {
                    System.Console.WriteLine("\nВы ввели пустую строку! Попробуйте еще раз!");
                }
                else if (input_end.Trim() == "0")
                {
                    return (false, DateTime.MinValue, DateTime.MaxValue);
                }
                else if (!DateTime.TryParseExact(input_end.Trim(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data_end_temp))
                {
                    System.Console.WriteLine("\nВы ввели дату в неправильном формате!");
                }
                else
                {
                    data_end = data_end_temp;
                    break;
                }
            }

            if (data_start > data_end)
            {
                System.Console.WriteLine("\nДата начала НЕ может превышать даты конца! \nСделайте сводки заново с правильным интервалом!");
                return (false, DateTime.MinValue, DateTime.MaxValue);
            }
            return (true, data_start, data_end);
        }

        public BankAccount? SelectAccount(IAccountRepository accountRepo)
        {
            ShowMyAccounts(accountRepo);

            var bankAccounts = accountRepo.GetAllAccounts();

            if (bankAccounts.Count == 0)
            {
                return null;
            }

            while (true)
            {
                System.Console.Write("Введите ID счета для выбора аккаунта (0 - назад): ");
                var cmd_id_inside = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(cmd_id_inside))
                {
                    System.Console.WriteLine("Вы ничего не ввели! Попробуйте еще раз!");
                }
                else if (cmd_id_inside.Trim() == "0")
                {
                    return null;
                }
                else if (!Guid.TryParse(cmd_id_inside.Trim(), out var id))
                {
                    System.Console.WriteLine("Неверный формат ID (возможно, произошло неточное копирование). Попробуйте еще раз");
                }
                else
                {
                    foreach (var el in bankAccounts)
                    {
                        if (el.AccountId == id)
                        {
                            Console.Clear();
                            System.Console.WriteLine("Счет найден!\n");
                            System.Console.WriteLine($"Имя: {el.Name}");
                            System.Console.WriteLine($"Баланс: {el.Balance:F2}\n");
                            return el;
                        }
                    }
                    System.Console.WriteLine("Счета с таким ID нет! Попробуйте еще раз!");
                }
            }
        }

        public Category? SelectCategory(ICategoryRepository categoryRepo)
        {
            var categories = categoryRepo.GetAllCategories();

            if (categories.Count == 0)
            {
                System.Console.WriteLine("Категории отсутствуют");
                return null;
            }
            System.Console.WriteLine("Доступные категории: ");
            foreach (var el in categories)
            {
                System.Console.WriteLine($"Название: {el.Name}, Тип: {el.Type}");
            }

            while (true)
            {
                System.Console.Write("\nВведите название категории операции (0 - назад): ");
                var input_string = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input_string))
                {
                    System.Console.WriteLine("\nВы ничего не ввели! Попробуйте еще раз!");
                }
                else if (input_string.Trim() == "0")
                {
                    return null;
                }
                else
                {
                    var result = categoryRepo.FindCategoryByName(input_string);
                    if (result != null)
                    {
                        Console.Clear();
                        System.Console.WriteLine("\nКатегория найдена и выбрана!");
                        System.Console.WriteLine($"\nНазвание: {result.Name}");
                        System.Console.WriteLine($"ID категории: {result.CategoryId}");
                        return result;
                    }
                    System.Console.WriteLine("\nТакой категории не существует! Попробуйте еще раз!");
                }
            }
        }

        public (bool success, Guid id) ReadGuid(BankAccount account)
        {
            while (true)
            {
                System.Console.Write("\nВведите ID операции для выбранного аккаунта (0 - назад): ");
                var cmd_id_inside = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(cmd_id_inside))
                {
                    System.Console.WriteLine("Вы ничего не ввели! Попробуйте еще раз!");
                }
                else if (cmd_id_inside.Trim() == "0")
                {
                    return (false, Guid.NewGuid());
                }
                else if (!Guid.TryParse(cmd_id_inside.Trim(), out var id))
                {
                    System.Console.WriteLine("Неверный формат ID (возможно, произошло неточное копирование). Попробуйте еще раз");
                }
                else
                {
                    return (true, id);
                }
            }
        }
    }
}