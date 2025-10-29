using BankAccountSpace;
using CategorySpace;
using OperationSpace;

namespace Hse_bank
{
    internal class Program
    {
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
                    case "0":
                        return;
                    default:
                        System.Console.WriteLine("Неизвестная команда!");
                        break;
                }
            }
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
            throw new NotImplementedException();
        }
    }
}