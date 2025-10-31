using Application;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.InteropServices;

namespace Facades
{
    public class ImportExportFacade
    {
        private IDataImporter? _importer;
        private IDataExporter? _exporter;
        private DataRestoreService? _restorer;
        private IAccountRepository? accountRepo;
        private ICategoryRepository? categoryRepo;
        public ImportExportFacade(IDataImporter dataImporter, IDataExporter exporter, DataRestoreService dataRestore, IAccountRepository accountRepository, ICategoryRepository categoryRepository)
        {
            _importer = dataImporter;
            _restorer = dataRestore;
            _exporter = exporter;
            accountRepo = accountRepository;
            categoryRepo = categoryRepository;
        }

        public void ImportData()
        {
            if (_importer == null || _restorer == null)
            {
                System.Console.WriteLine("Произошла ошибка!");
                return;
            }

            Console.Clear();
            System.Console.WriteLine("Давайте импортируем файлы!\n");

            System.Console.Write("Выберите, откуда будем импортировать (1 - Json) (0 - назад): ");
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
                            var snapshot = _importer.Import(path);
                            _restorer.RestoreFromSnapshot(snapshot);

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

        public void ExportData()
        {
            if (_exporter == null || categoryRepo == null || accountRepo == null)
            {
                System.Console.WriteLine("Произошла ошибка!");
                return;
            }
            var categories = categoryRepo.GetAllCategories();
            var bankAccounts = accountRepo.GetAllAccounts();

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
                        try
                        {
                            _exporter.Export(snapshot, filePath);
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

        private string? FilePathHelper()

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

        private string NormalizeUserPath(string input)
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

        private bool FileOrDirectoryExists(string normalizedPath) =>
            File.Exists(normalizedPath) || Directory.Exists(normalizedPath);

    }
}