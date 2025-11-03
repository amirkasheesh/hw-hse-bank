using Infrastructure;
using Interfaces;
using Application;
using Facades;
using Microsoft.Extensions.DependencyInjection;
using Commands;
using AccountCommands;
using AnalyticsCommands;
using CategoryCommands;
using DataCommands;
using Decorator;

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
            ImportExportFacade importExportFacade = new ImportExportFacade(importer, exporter, restorer, _accountRepo, _categoryRepo);

            var commands = new List<(string key, ICommand cmd)>
            {
                ("1", new TimedCommand(new SafeCommand(new CreateAccountCommand(accountsFacade)))),
                ("2", new TimedCommand(new SafeCommand(new AddOperationCommand(accountsFacade)))),
                ("3", new TimedCommand(new SafeCommand(new DeleteAccountCommand(accountsFacade)))),
                ("4", new TimedCommand(new SafeCommand(new DeleteOperationCommand(accountsFacade)))),
                ("5", new TimedCommand(new SafeCommand(new ShowExtractCommand(analyticsFacade)))),
                ("6", new TimedCommand(new SafeCommand(new ShowResultsCommand(analyticsFacade)))),
                ("7", new TimedCommand(new SafeCommand(new ShowSummaryByCategoryCommand(analyticsFacade)))),
                ("8", new TimedCommand(new SafeCommand(new CheckBalanceCommand(analyticsFacade)))),
                ("9", new TimedCommand(new SafeCommand(new AddSomeCategoryCommand(categoriesFacade)))),
                ("10", new TimedCommand(new SafeCommand(new DeleteCategoryCommand(categoriesFacade)))),
                ("11", new TimedCommand(new SafeCommand(new ExportDataCommand(importExportFacade)))),
                ("12", new TimedCommand(new SafeCommand(new ImportDataCommand(importExportFacade)))),
                ("13", new TimedCommand(new SafeCommand(new ShowAccountsCommand(accountsFacade))))
            };



            while (true)
            {
                System.Console.WriteLine("\n\tБанк Высшей Школы Экономики");
                foreach (var el in commands)
                {
                    System.Console.WriteLine($"{el.key}) {el.cmd.Name}");
                }
                System.Console.WriteLine("0) Выход.");

                System.Console.Write("\nВаш выбор (0 - 13): ");
                var command_input = Console.ReadLine();
                System.Console.WriteLine();

                if (string.IsNullOrEmpty(command_input))
                {
                    System.Console.WriteLine("\nВы ничего не ввели! Попробуйте еще раз!\n");
                }
                else if (command_input == "0")
                {
                    return;
                }
                else if (commands.FirstOrDefault(c => c.key == command_input).cmd != null)
                {
                    commands.FirstOrDefault(c => c.key == command_input).cmd.Execute();
                }
                else
                {
                    System.Console.WriteLine("\nТакой команды нет! Попробуйте еще раз!\n");
                }
            }
        }
    }
}