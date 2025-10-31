using CategorySpace;

namespace Application
{
    public sealed class CsvDataImporter : BaseFileImporter
    {
        public override BankDataSnapshot Parse(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new InvalidOperationException("CSV пустой или не содержит данных");

            var lines = content
                .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();

            if (lines.Count == 0)
                throw new InvalidOperationException("CSV не содержит строк");

            int startIndex = 0;
            var header = lines[0];
            if (header.IndexOf("CategoryId", StringComparison.OrdinalIgnoreCase) >= 0 ||
                header.IndexOf("Name", StringComparison.OrdinalIgnoreCase) >= 0 ||
                header.IndexOf("Type", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                startIndex = 1;
            }

            var categories = new List<Category>();
            for (int i = startIndex; i < lines.Count; i++)
            {
                var line = lines[i].Trim();
                if (line.Length == 0) continue;

                var parts = line.Split(';');
                if (parts.Length < 3)
                    throw new InvalidOperationException($"CSV: строка {i + 1} имеет неверный формат. Ожидается 3 столбца: CategoryId;Name;Type");

                if (!Guid.TryParse(parts[0].Trim(), out var categoryId))
                    throw new InvalidOperationException($"CSV: строка {i + 1}: некорректный GUID категории: '{parts[0]}'");

                var name = parts[1].Trim();
                if (string.IsNullOrWhiteSpace(name))
                    throw new InvalidOperationException($"CSV: строка {i + 1}: пустое имя категории.");

                var typeRaw = parts[2].Trim();
                OperationType type;
                if (typeRaw.Equals("income", StringComparison.OrdinalIgnoreCase))
                    type = OperationType.Income;
                else if (typeRaw.Equals("expense", StringComparison.OrdinalIgnoreCase))
                    type = OperationType.Expense;
                else
                    throw new InvalidOperationException($"CSV: строка {i + 1}: неизвестный тип '{typeRaw}'. Допустимо: Income или Expense");

                categories.Add(new Category
                {
                    CategoryId = categoryId,
                    Name = name,
                    Type = type
                });
            }
            return new BankDataSnapshot
            {
                Accounts = new List<AccountWithOperations>(),
                Categories = categories
            };
        }
    }
}
