using System.Globalization;
using BankAccountSpace;
using CategorySpace;
using Infrastructure;
using Interfaces;
using Application;
using Facades;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;
using OperationSpace;

namespace HseBankSpace
{
    internal class Program
    {
        private static IAccountRepository? _accountRepo;
        private static ICategoryRepository? _categoryRepo;
        private static IDataImporter? importer;
        private static IDataExporter? exporter;
        private static DataRestoreService? restorer;

        public static void Main()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<IAccountRepository, InMemoryAccountRepository>();
            services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
            services.AddSingleton<IDataImporter, JsonDataImporter>();
            services.AddSingleton<IDataExporter, JsonDataExporter>();
            services.AddSingleton<DataRestoreService>();

            var serviceProvider = services.BuildServiceProvider();

            importer = serviceProvider.GetService<IDataImporter>();
            exporter = serviceProvider.GetService<IDataExporter>();
            restorer = serviceProvider.GetService<DataRestoreService>();

            _accountRepo = serviceProvider.GetService<IAccountRepository>();
            _categoryRepo = serviceProvider.GetService<ICategoryRepository>();


            if (_categoryRepo == null || _accountRepo == null)
            {
                System.Console.WriteLine("Репозитории не настроены!");
                return;
            }

            CategoriesFacade categoriesFacade = new CategoriesFacade(_categoryRepo, _accountRepo);
            AccountsFacade accountsFacade = new AccountsFacade(_categoryRepo, _accountRepo);
            AnalyticsFacade analyticsFacade = new AnalyticsFacade(_categoryRepo, _accountRepo);

            Console.OutputEncoding = System.Text.Encoding.UTF8;

            if (importer == null || exporter == null || restorer == null)
            {
                System.Console.WriteLine("Произошла ошибка!");
                return;
            }
            ImportExportFacade importExportFacade = new ImportExportFacade(importer, exporter, restorer);

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
                Console.WriteLine("10) Добавить/посмотреть категорию;");
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
                        accountsFacade.CreateAccount();
                        break;
                    case "2":
                        accountsFacade.AddSomeOperation();
                        break;
                    case "3":
                        analyticsFacade.GiveExtractByPeriod();
                        break;
                    case "4":
                        analyticsFacade.ResultsByPeriod();
                        break;
                    case "5":
                        analyticsFacade.GiveSummaryByCategoryForPeriod();
                        break;
                    case "6":
                        analyticsFacade.CheckYourBalanceFunc();
                        break;
                    case "7":
                        Console.Clear();
                        ConsoleInputHelper shower = new ConsoleInputHelper();
                        shower.ShowMyAccounts(_accountRepo);
                        break;
                    case "8":
                        importExportFacade.ExportData(_categoryRepo, _accountRepo);
                        break;
                    case "9":
                        importExportFacade.ImportData();
                        break;
                    case "10":
                        categoriesFacade.AddSomeCategory();
                        break;
                    case "11":
                        accountsFacade.DeleteOneBankAccount();
                        break;
                    case "12":
                        accountsFacade.DeleteOperationInAccount();
                        break;
                    case "13":
                        categoriesFacade.DeleteOneCategory();
                        break;
                    case "0":
                        return;
                    default:
                        System.Console.WriteLine("Неизвестная команда!");
                        break;
                }
            }
        }
    }
}