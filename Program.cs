using BankAccountSpace;
using CategorySpace;
using OperationSpace;

namespace HseBankSpace
{
    internal class Program
    {
        internal static List<BankAccount> bankAccounts = new List<BankAccount>();

        internal static readonly List<Category> categories = new List<Category>
        {
            new Category {CategoryId = Guid.NewGuid(), Name = "Зарплата", Type = OperationType.Income },
            new Category {CategoryId = Guid.NewGuid(), Name = "Кэшбек", Type = OperationType.Income},

            new Category {CategoryId = Guid.NewGuid(), Name = "Рестораны/Кафе", Type = OperationType.Expense},
            new Category {CategoryId = Guid.NewGuid(), Name = "Транспорт", Type = OperationType.Expense},
            new Category {CategoryId = Guid.NewGuid(), Name = "Здоровье", Type = OperationType.Expense},
            new Category {CategoryId = Guid.NewGuid(), Name = "Развлечения", Type = OperationType.Expense}
        };

        public static void Main()
        {
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
                System.Console.WriteLine("7) Посмотреть мои счета;");
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
                        ShowMyAccounts();
                        break;
                    case "0":
                        return;
                    default:
                        System.Console.WriteLine("Неизвестная команда!");
                        break;
                }
            }
        }

        private static void ShowMyAccounts()
        {
            Console.Clear();
            if (bankAccounts.Count == 0)
            {
                System.Console.WriteLine("Счетов еще нет!");
                return;
            }
            System.Console.WriteLine("\tВаши доступные счета: \n\n");
            int res = 0;
            for (int i = 0; i < bankAccounts.Count; ++i)
            {
                System.Console.WriteLine($"Счет №{i + 1}:");
                System.Console.WriteLine($"Имя: {bankAccounts[i].Name}");
                System.Console.WriteLine($"Уникальный идентификатор: {bankAccounts[i].AccountId}");
                System.Console.WriteLine($"Баланс: {bankAccounts[i].Balance}");
                System.Console.WriteLine();
                ++res;
            }
            System.Console.WriteLine($"Количество счетов: {res}");
        }

        private static void CheckYourBalanceFunc()
        {
            throw new NotImplementedException();
        }

        private static void GiveSummaryByCategoryForPeriod()
        {
            throw new NotImplementedException();
        }

        private static void ResultsByPeriod()
        {
            throw new NotImplementedException();
        }

        private static void GiveExtractByPeriod()
        {
            throw new NotImplementedException();
        }

        private static void AddSomeOperation()
        {
            throw new NotImplementedException();
        }

        private static void CreateAccount()
        {
            Console.Clear();
            System.Console.WriteLine("Давайте создадим счет!");

            string input;
            while (true)
            {
                System.Console.Write("Введите свое имя: ");
                var input_inside = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input_inside))
                {
                    System.Console.WriteLine("Вы ничего не ввели! Попробуйте еще раз!");
                }
                else if (bankAccounts.Select(account => account.Name.ToLower()).Contains(input_inside.Trim().ToLower()))
                {
                    System.Console.WriteLine("Счет с таким именем уже есть! Попробуйте еще раз!");
                }
                else if (input_inside[0] != char.ToUpper(input_inside[0]))
                {
                    System.Console.WriteLine("Имя должно быть с большой буквы! Попробуйте еще раз!");
                }
                else if (!input_inside.Any(char.IsLetter))
                {
                    System.Console.WriteLine("В имени счета должна быть хотя бы одна буква! Попробуйте еще раз!");
                }
                else
                {
                    input = input_inside.Trim();
                    break;
                }
            }
            
            Guid account_id = Guid.NewGuid();
            BankAccount account = new BankAccount { AccountId = account_id, Name = input }; // Стартовый баланс = 0
            bankAccounts.Add(account);

            System.Console.WriteLine("Ваш аккаунт успешно создан!\n");
            System.Console.WriteLine("Ваши данные:");
            System.Console.WriteLine($"Имя: {input}");
            System.Console.WriteLine($"Уникальный идентификатор: {account_id}");
        }
    }
}