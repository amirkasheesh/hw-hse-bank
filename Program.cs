using System.Globalization;
using BankAccountSpace;
using CategorySpace;
using Infrastructure;
using Interfaces;
using Application;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Runtime.InteropServices;
using OperationSpace;

namespace HseBankSpace
{
    internal class Program
    {
        private static IAccountRepository? _accountRepo;
        private static ICategoryRepository? _categoryRepo;

        private static IDataImporter? importer;
        private static DataRestoreService? restorer;

        public static void Main()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IAccountRepository, InMemoryAccountRepository>();
            services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
            services.AddSingleton<IDataImporter, JsonDataImporter>();
            services.AddSingleton<DataRestoreService>();

            var serviceProvider = services.BuildServiceProvider();

            _accountRepo = serviceProvider.GetService<IAccountRepository>();
            _categoryRepo = serviceProvider.GetService<ICategoryRepository>();

            importer = serviceProvider.GetService<IDataImporter>();
            restorer = serviceProvider.GetService<DataRestoreService>();
            


            Console.OutputEncoding = System.Text.Encoding.UTF8;

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("\tБанк Высшей Школы Экономики");
                Console.WriteLine("1) Создать счет;");
                Console.WriteLine("2) Добавить операцию;");
                Console.WriteLine("3) Выписка за период;");
                Console.WriteLine("4) Итоги (доход/расход/итог) за период;");
                Console.WriteLine("5) Сводка по категориям за период;");
                Console.WriteLine("6) Самопроверка баланса. Пересчитать из операций;");
                Console.WriteLine("7) Посмотреть мои счета;");
                Console.WriteLine("8) Экспорт данных;");
                Console.WriteLine("9) Импорт данных;");
                Console.WriteLine("10) Добавить категорию;");
                Console.WriteLine("11) Удалить счет;");
                Console.WriteLine("12) Удалить операцию у счета;");
                Console.WriteLine("13) Удалить категорию");
                Console.WriteLine("0) Выход.");
                Console.Write("Ваш ответ: ");

                var command = Console.ReadLine();
                System.Console.WriteLine();

                switch (command)
                {
                    case "1":
                        CreateAccount();
                        break;
                    case "2":
                        AddSomeOperation();
                        break;
                    case "3":
                        GiveExtractByPeriod();
                        break;
                    case "4":
                        ResultsByPeriod();
                        break;
                    case "5":
                        GiveSummaryByCategoryForPeriod();
                        break;
                    case "6":
                        CheckYourBalanceFunc();
                        break;
                    case "7":
                        Console.Clear();
                        ShowMyAccounts();
                        break;
                    case "8":
                        ExportData();
                        break;
                    case "9":
                        ImportData();
                        break;
                    case "10":
                        AddSomeCategory();
                        break;
                    case "11":
                        DeleteOneBankAccount();
                        break;
                    case "12":
                        DeleteOperationInAccount();
                        break;
                    case "13":
                        DeleteOneCategory();
                        break;
                    case "0":
                        return;
                    default:
                        System.Console.WriteLine("Неизвестная команда!");
                        break;
                }
            }
        }

        private static void DeleteOneCategory()
        {
            Console.Clear();
            if (_categoryRepo == null || _accountRepo == null)
            {
                System.Console.WriteLine("\nНе получается получить доступ к данным!");
                return;
            }
            System.Console.WriteLine("Давайте удалим категорию!");

            System.Console.WriteLine("Список доступных:");

            var categories = _categoryRepo.GetAllCategories();
            for (int i = 0; i < categories.Count; ++i)
            {
                System.Console.WriteLine($"\nКатегория №{i + 1}");
                System.Console.WriteLine($"Название: {categories[i].Name};");
                System.Console.WriteLine($"Id категории: {categories[i].CategoryId};");
                System.Console.WriteLine($"Тип: {categories[i].Type}.");
            }

            while (true)
            {
                System.Console.Write("\nВведите(скопируйте) ID категории (0 - назад): ");
                var cmd_id_inside = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(cmd_id_inside))
                {
                    System.Console.WriteLine("Вы ничего не ввели! Попробуйте еще раз!");
                }
                else if (cmd_id_inside.Trim() == "0")
                {
                    return;
                }
                else if (!Guid.TryParse(cmd_id_inside.Trim(), out var id))
                {
                    System.Console.WriteLine("Неверный формат ID (возможно, произошло неточное копирование). Попробуйте еще раз");
                }
                else
                {
                    if (_accountRepo.CheckIfExistsCategoryById(id))
                    {
                        System.Console.WriteLine("\nНельзя удалить категорию, которая содержится в операциях! Выберите другую!");
                        continue;
                    }
                    if (_categoryRepo.ClearCategory(id) == 1)
                    {
                        System.Console.WriteLine("\nКатегория была удалена!");
                        return;
                    }
                    System.Console.WriteLine("Категории с таким ID нет! Попробуйте еще раз!");
                }
            }

        }

        private static void DeleteOperationInAccount()
        {
            Console.Clear();
            System.Console.WriteLine("Давайте удалим выбранную вами операцию из аккаунта:\n");

            var account = SelectAccount();
            if (account == null || _categoryRepo == null)
            {
                return;
            }

            System.Console.WriteLine("\nВот список операций этого аккаунта:");

            var operations = account.ShowOperationsByDate(DateTime.MinValue, DateTime.MaxValue);
            for (int i = 0; i < operations.Count; i++)
            {
                System.Console.WriteLine($"\nОперация №{i + 1}");
                System.Console.WriteLine($"Дата: {operations[i].Date};");
                System.Console.WriteLine($"Категория: {_categoryRepo.FindCategoryById(operations[i].CategoryId)?.Name ?? "Категория не найдена"};");
                System.Console.WriteLine($"Сумма: {operations[i].Amount};");
                System.Console.WriteLine($"Id операции: {operations[i].OperationId};");
                System.Console.WriteLine($"Описание: {operations[i].Description}.");
            }
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
                    return;
                }
                else if (!Guid.TryParse(cmd_id_inside.Trim(), out var id))
                {
                    System.Console.WriteLine("Неверный формат ID (возможно, произошло неточное копирование). Попробуйте еще раз");
                }
                else
                {
                    if (account.ClearOperation(id) == 1)
                    {
                        System.Console.WriteLine("\nОперация была удалена!");
                        return;
                    }
                    System.Console.WriteLine("Операции с таким ID нет! Попробуйте еще раз!");
                }
            }
        }

        private static void DeleteOneBankAccount()
        {
            Console.Clear();
            System.Console.WriteLine("Давайте удалим выбранный вами аккаунт!\n");

            var account = SelectAccount();
            if (account == null || _accountRepo == null)
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
                    _accountRepo.Clear(account.AccountId);
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

        private static void AddSomeCategory()
        {
            Console.Clear();
            if (_categoryRepo == null)
            {
                System.Console.WriteLine("\nНе получается получить доступ к категориям!");
                return;
            }
            System.Console.WriteLine("Давайте добавим новую категорию!\n");

            System.Console.WriteLine("Список доступных:");
            var categories = _categoryRepo.GetAllCategories();
            for (int i = 0; i < categories.Count; ++i)
            {
                System.Console.WriteLine($"\nКатегория №{i + 1}");
                System.Console.WriteLine($"Название: {categories[i].Name}");
                System.Console.WriteLine($"Тип: {categories[i].Type}");
            }

            string name;
            while (true)
            {
                System.Console.Write("\nВведите название категории (0 - назад): ");
                string? cmd = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(cmd))
                {
                    System.Console.WriteLine("\nВы ничего не ввели! Попробуйте еще раз!");
                }
                else if (cmd.Trim() == "0")
                {
                    return;
                }
                else if (_categoryRepo.FindCategoryByName(cmd) != null)
                {
                    System.Console.WriteLine("Категория с таким именем уже есть! Попробуйте еще раз!");
                }
                else
                {
                    System.Console.WriteLine("Название записано!");
                    name = cmd;
                    break;
                }
            }

            OperationType type = new OperationType();
            while (true)
            {
                System.Console.Write("\nВведите тип категории (расход/доход) (0 - назад): ");
                string? cmd1 = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(cmd1))
                {
                    System.Console.WriteLine("\nВы ничего не ввели! Попробуйте еще раз!");
                }
                else if (cmd1.Trim() == "0")
                {
                    return;
                }
                else if (cmd1.Trim().ToLower() == "расход")
                {
                    type = OperationType.Expense;
                    break;
                }
                else if (cmd1.Trim().ToLower() == "доход")
                {
                    type = OperationType.Income;
                    break;
                }
                else
                {
                    System.Console.WriteLine("\nНужно четко ввести по буквам расход ИЛИ доход! Попробуйте еще раз!");
                }
            }

            Guid guid = Guid.NewGuid();
            Category category = new Category {CategoryId = guid, Type = type, Name = name};

            _categoryRepo.AddNewCategory(category);
            System.Console.WriteLine("\nКатегория была добавлена!");
            return;
        }

        private static void ImportData()
        {
            if (importer == null || restorer == null)
            {
                System.Console.WriteLine("Произошла ошибка!");
                return;
            }

            Console.Clear();
            System.Console.WriteLine("Давайте импортируем файлы!\n");
            
            System.Console.Write("Выберите, откуда будем экспортировать (1 - Json) (0 - назад): ");
            var cmd = Console.ReadLine();

            while (true)
            {
                switch (cmd)
                {
                    case "0":
                        return;
                    case "1":
                        var path = FilePathHelper();
                        if (path == null)
                        {
                            System.Console.WriteLine("Произошла ошибка c путем!");
                            return;
                        }
                        try
                        {
                            var snapshot = importer.Import(path);
                            restorer.RestoreFromSnapshot(snapshot);

                            Console.WriteLine("\nИмпорт завершён!");
                            return;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            Console.WriteLine("\nВы указали только папку или у вас нет прав. Введите путь вида: /.../docs/data.json");
                            return;
                        }
                        catch (FileNotFoundException)
                        {
                            System.Console.WriteLine("Файл не был найден!");
                            return;
                        }
                        catch (System.Text.Json.JsonException)
                        {
                            System.Console.WriteLine("Json-файл либо поломан, либо имеет неподходящую структуру!");
                            return;
                        }
                    default:
                        System.Console.WriteLine("Неизвестная команда!");
                        return;
                }
            }
        }

        private static void ExportData()
        {
            if (_categoryRepo == null || _accountRepo == null)
            {
                System.Console.WriteLine("Произошла ошибка!");
                return;
            }

            var categories = _categoryRepo.GetAllCategories();
            var bankAccounts = _accountRepo.GetAllAccounts();

            BankDataSnapshot snapshot = new BankDataSnapshot();

            foreach (var el in bankAccounts)
            {
                snapshot.Accounts.Add(
                    new AccountWithOperations
                    {
                        AccountId = el.AccountId,
                        Name = el.Name,
                        Balance = el.Balance,
                        Operations = el.ShowOperationsByDate(DateTime.MinValue, DateTime.MaxValue)
                    }
                );
            }
            snapshot.Categories = categories;

            Console.Clear();
            System.Console.WriteLine("Давайте экспортируем данные!\n");
            System.Console.Write("Выберите, куда будем экспортировать (1 - Json) (0 - назад): ");
            var cmd = Console.ReadLine();

            while (true)
            {
                switch (cmd)
                {
                    case "0":
                        return;
                    case "1":
                        var filePath = FilePathHelper();
                        if (filePath == null)
                        {
                            return;
                        }

                        JsonDataExporter exporter = new JsonDataExporter();
                        try
                        {
                            exporter.Export(snapshot, filePath);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            Console.WriteLine("\nВы указали только папку или у вас нет прав. Введите путь вида: /.../docs/data.json");
                            return;
                        }
                        catch (FileNotFoundException)
                        {
                            System.Console.WriteLine("Файл не был найден!");
                            return;
                        }
                        catch (System.Text.Json.JsonException)
                        {
                            System.Console.WriteLine("Json-файл либо поломан, либо имеет неподходящую структуру!");
                            return;
                        }
                        System.Console.WriteLine("\nЭкспорт завершен!");
                        return;
                    default:
                        System.Console.WriteLine("Неизвестная команда!");
                        return;
                }
            }
        }

        private static string? FilePathHelper()

        {
            Console.Clear();
            Console.Write("Введите путь к файлу (пример: /Users/amir/.../data.json): ");
            var raw = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(raw))
            {
                Console.WriteLine("Ничего не введено!");
                return null;
            }

            var path = NormalizeUserPath(raw.Trim());

            var dir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
            {
                Console.WriteLine("Такой папки не существует! Сначала укажи существующую папку");
                return null;
            }

            return path;
        }
        
        public static string NormalizeUserPath(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Путь пуст");
            }

            var path = input.Trim().Trim('"', '\'');

            if (path == "~" || path.StartsWith("~/"))
            {
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                path = Path.Join(home, path.Substring(1));
            }

            path = Environment.ExpandEnvironmentVariables(path);

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (path.Contains('\\') && !path.Contains('/'))
                    path = path.Replace('\\', '/');
            }

            var invalid = Path.GetInvalidPathChars();
            if (path.Any(ch => invalid.Contains(ch)))
            {
                throw new ArgumentException("Путь содержит недопустимые символы!");
            }

            path = Path.GetFullPath(path);

            path = Path.TrimEndingDirectorySeparator(path);

            return path;
        }

        public static bool FileOrDirectoryExists(string normalizedPath) =>
            File.Exists(normalizedPath) || Directory.Exists(normalizedPath);

        private static void ShowMyAccounts()
        {
            if (_accountRepo == null)
            {
                return;
            }
            var bankAccounts = _accountRepo.GetAllAccounts();

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

        private static void CheckYourBalanceFunc()
        {
            Console.Clear();
            Console.WriteLine("Давайте проверим баланс!\n");

            var account = SelectAccount();
            if (account == null)
            {
                return;
            }

            var ops = account.ShowOperationsByDate(DateTime.MinValue, DateTime.MaxValue);

            if (ops.Count == 0)
            {
                System.Console.WriteLine("Операций за весь период нет!");
                return;
            }

            decimal income = 0, expense = 0;

            foreach (var op in ops)
            {
                if (op.Type == OperationType.Income)
                {
                    income += op.Amount;
                }
                else if (op.Type == OperationType.Expense)
                {
                    expense += op.Amount;
                }
            }

            decimal computed = income - expense;
            decimal stored = account.Balance;
            decimal diff = computed - stored;

            Console.WriteLine($"Счёт: {account.Name} ({account.AccountId})");
            Console.WriteLine($"Текущий баланс: {stored:F2}");
            Console.WriteLine($"Баланс по операциям: {computed:F2}");
            Console.WriteLine($"Доходы: {income:F2}; Расходы: {expense:F2}; Операций: {ops.Count}");

            if (diff == 0)
            {
                Console.WriteLine("Баланс сходится");
            }
            else
            {
                Console.WriteLine($"Баланс НЕ сходится. Разница: {diff:F2}");
            }
            return;
        }


        private static void GiveSummaryByCategoryForPeriod()
        {
            Console.Clear();
            System.Console.WriteLine("Давайте выберем временной промежуток для сводки операций по категориям!");

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
                    return;
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
                    return;
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
                return;
            }

            Console.Clear();
            BankAccount? chosen_account = SelectAccount();
            if (chosen_account == null)
            {
                return;
            }
            var result = chosen_account.GetInfByCategoryForPeriod(data_start, data_end);

            if (result.Count == 0)
            {
                System.Console.WriteLine("За данный период не было ни одной операции! (Вероятно, вы выбрали не тот интервал)");
                return;
            }

            Console.Clear();
            System.Console.WriteLine($"Сводка операций по категориям за период с {data_start:dd.MM.yyyy} по {data_end:dd.MM.yyyy}:");

            if (_categoryRepo == null)
            {
                return;
            }

            bool flag = true;

            decimal sum_income = 0;
            decimal sum_expense = 0;
            int count = 0;
            if (result.Count == 1)
            {
                string name_cat = "";
                var cat = _categoryRepo.FindCategoryById(result[0].CategoryId);
                if (cat == null)
                {
                    return;
                }

                name_cat = cat.Name;
                if (string.IsNullOrEmpty(name_cat))
                {
                    System.Console.WriteLine("Категория не нашлась!");
                    return;
                }
                System.Console.WriteLine($"\n\nКатегория {name_cat}:");

                System.Console.WriteLine($"\nТип: {result[0].Type};");
                System.Console.WriteLine($"Дата: {result[0].Date:dd.MM.yyyy};");
                System.Console.WriteLine($"Сумма: {result[0].Amount:F2};");
                System.Console.WriteLine($"Описание: {result[0].Description ?? "Описания нет"};");
                if (result[0].Type == OperationType.Income)
                {
                    sum_income += result[0].Amount;
                }
                else
                {
                    sum_expense += result[0].Amount;
                }
                System.Console.WriteLine($"Итого по категории: сумма доходов = {sum_income}; сумма расходов = {sum_expense}; кол-во = {1}");
                return;
            }

            for (int i = 0; i < result.Count - 1; i++)
            {
                if (flag)
                {
                    flag = false;
                    string name_cat = "";
                    var cat = _categoryRepo.FindCategoryById(result[i].CategoryId);
                    if (cat == null)
                    {
                        return;
                    }

                    name_cat = cat.Name;
                    if (string.IsNullOrEmpty(name_cat))
                    {
                        System.Console.WriteLine("Категория не нашлась!");
                        return;
                    }
                    System.Console.WriteLine($"\n\nКатегория {name_cat}:");
                }

                if (result[i].CategoryId == result[i + 1].CategoryId)
                {
                    System.Console.WriteLine($"\nТип: {result[i].Type};");
                    System.Console.WriteLine($"Дата: {result[i].Date:dd.MM.yyyy};");
                    System.Console.WriteLine($"Сумма: {result[i].Amount:F2};");
                    System.Console.WriteLine($"Описание: {result[i].Description ?? "Описания нет"};");
                    ++count;
                    if (result[i].Type == OperationType.Income)
                    {
                        sum_income += result[i].Amount;
                    }
                    else
                    {
                        sum_expense += result[i].Amount;
                    }

                    if (i == result.Count - 2)
                    {
                        ++i;
                        System.Console.WriteLine($"\nТип: {result[i].Type};");
                        System.Console.WriteLine($"Дата: {result[i].Date:dd.MM.yyyy};");
                        System.Console.WriteLine($"Сумма: {result[i].Amount:F2};");
                        System.Console.WriteLine($"Описание: {result[i].Description ?? "Описания нет"};");
                        ++count;
                        if (result[i].Type == OperationType.Income)
                        {
                            sum_income += result[i].Amount;
                        }
                        else
                        {
                            sum_expense += result[i].Amount;
                        }
                        System.Console.WriteLine($"Итого по категории: сумма доходов = {sum_income}; сумма расходов = {sum_expense}; кол-во = {count}");
                        sum_expense = 0;
                        sum_income = 0;
                        count = 0;
                    }
                }
                else
                {
                    System.Console.WriteLine($"\nТип: {result[i].Type};");
                    System.Console.WriteLine($"Дата: {result[i].Date:dd.MM.yyyy};");
                    System.Console.WriteLine($"Сумма: {result[i].Amount:F2};");
                    System.Console.WriteLine($"Описание: {result[i].Description ?? "Описания нет"};");
                    ++count;
                    if (result[i].Type == OperationType.Income)
                    {
                        sum_income += result[i].Amount;
                    }
                    else
                    {
                        sum_expense += result[i].Amount;
                    }
                    System.Console.WriteLine($"Итого по категории: сумма доходов = {sum_income}; сумма расходов = {sum_expense}; кол-во = {count}");
                    sum_expense = 0;
                    sum_income = 0;
                    count = 0;

                    flag = true;
                    if (i == result.Count - 2)
                    {
                        ++i;
                        string name_cat = "";
                        var cat = _categoryRepo.FindCategoryById(result[i].CategoryId);
                        if (cat == null)
                        {
                            return;
                        }

                        name_cat = cat.Name;
                        if (string.IsNullOrEmpty(name_cat))
                        {
                            System.Console.WriteLine("Категория не нашлась!");
                            return;
                        }
                        System.Console.WriteLine($"\n\nКатегория {name_cat}:");

                        System.Console.WriteLine($"\nТип: {result[i].Type};");
                        System.Console.WriteLine($"Дата: {result[i].Date:dd.MM.yyyy};");
                        System.Console.WriteLine($"Сумма: {result[i].Amount:F2};");
                        System.Console.WriteLine($"Описание: {result[i].Description ?? "Описания нет"};");
                        ++count;

                        if (result[i].Type == OperationType.Income)
                        {
                            sum_income += result[i].Amount;
                        }
                        else
                        {
                            sum_expense += result[i].Amount;
                        }
                        System.Console.WriteLine($"Итого по категории: сумма доходов = {sum_income}; сумма расходов = {sum_expense}; кол-во = {count}");
                        sum_expense = 0;
                        sum_income = 0;
                        count = 0;
                    }
                }
            }
            return;
        }

        private static void ResultsByPeriod()
        {
            Console.Clear();
            System.Console.WriteLine("Давайте выберем временной промежуток для итогов!");

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
                    return;
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
                    return;
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
                System.Console.WriteLine("\nДата начала НЕ может превышать даты конца! \nСделайте итоги заново с правильным интервалом!");
                return;
            }

            Console.Clear();
            BankAccount? chosen_account = SelectAccount();
            if (chosen_account == null)
            {
                return;
            }
            var result = chosen_account.CalculateTotalsByDate(data_start, data_end);

            if (result.income == 0 && result.expense == 0 && result.total == 0)
            {
                System.Console.WriteLine("За данный период не было ни одной операции! (Вероятно, вы выбрали не тот интервал)");
                return;
            }

            System.Console.WriteLine($"Итоги операций за период с {data_start:dd.MM.yyyy} по {data_end:dd.MM.yyyy}:\n");

            System.Console.WriteLine($"Общий доход: {result.income:F2}");
            System.Console.WriteLine($"Общий расход: {result.expense:F2}");
            System.Console.WriteLine($"Итог: {result.total:F2}");
            return;
        }

        private static void GiveExtractByPeriod()
        {
            Console.Clear();
            System.Console.WriteLine("Давайте выберем временной промежуток для выписки!");

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
                    return;
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
                    return;
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
                System.Console.WriteLine("\nДата начала НЕ может превышать даты конца! \nСделайте выписку заново с правильным интервалом!");
                return;
            }

            Console.Clear();
            BankAccount? chosen_account = SelectAccount();
            if (chosen_account == null)
            {
                return;
            }
            var result = chosen_account.ShowOperationsByDate(data_start, data_end);

            if (result.Count == 0)
            {
                System.Console.WriteLine("За данный период не было ни одной операции! (Вероятно, вы выбрали не тот интервал)");
                return;
            }

            Console.Clear();
            System.Console.WriteLine($"Выписка по операциям аккаунта с именем {chosen_account.Name}:");
            System.Console.WriteLine($"Период с {data_start:dd.MM.yyyy} по {data_end:dd.MM.yyyy}");

            int count = 0;

            if (_categoryRepo == null)
            {
                return;
            }

            for (int i = 0; i < result.Count; ++i)
            {
                System.Console.WriteLine($"\nОперация №{i + 1}:");
                System.Console.WriteLine($"ID операции: {result[i].OperationId};");
                System.Console.WriteLine($"Дата: {result[i].Date:dd.MM.yyyy};");
                System.Console.WriteLine($"Тип операции: {result[i].Type};");
                string categ_name = "Категория не найдена";

                var cat = _categoryRepo.FindCategoryById(result[i].CategoryId);
                if (cat != null)
                {
                    categ_name = cat.Name;
                }
                System.Console.WriteLine($"Категория операции: {categ_name};");
                System.Console.WriteLine($"Сумма операции: {result[i].Amount:F2};");
                System.Console.WriteLine($"Описание: {result[i].Description ?? "Пусто"}.");
                count++;
            }
            System.Console.WriteLine($"\nИтого операций: {count}");
            return;
        }

        private static void AddSomeOperation()
        {
            Console.Clear();
            BankAccount? chosen_account = SelectAccount();
            if (chosen_account == null)
            {
                return;
            }

            Category? chosen_category = SelectCategory();
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

        private static BankAccount? SelectAccount()
        {
            ShowMyAccounts();

            if (_accountRepo == null)
            {
                return null;
            }

            var bankAccounts = _accountRepo.GetAllAccounts();

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

        private static Category? SelectCategory()
        {
            if (_categoryRepo == null)
            {
                return null;
            }

            var categories = _categoryRepo.GetAllCategories();

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
                    var result = _categoryRepo.FindCategoryByName(input_string);
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


        private static void CreateAccount()
        {
            Console.Clear();
            System.Console.WriteLine("Давайте создадим счет!");

            if (_accountRepo == null)
            {
                return;
            }
            var bankAccounts = _accountRepo.GetAllAccounts();
            string input;
            while (true)
            {
                System.Console.Write("\nВведите свое имя: ");
                var input_inside = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input_inside))
                {
                    System.Console.WriteLine("\nВы ничего не ввели! Попробуйте еще раз!");
                }
                else if (_accountRepo.CheckIfExistsByName(input_inside))
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
            _accountRepo.AddNewAccount(account);

            System.Console.WriteLine("Ваш аккаунт успешно создан!\n");
            System.Console.WriteLine("Ваши данные:");
            System.Console.WriteLine($"Имя: {input}");
            System.Console.WriteLine($"Уникальный идентификатор: {account_id}");
        }
    }
}