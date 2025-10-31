namespace Application
{
    public abstract class BaseFileImporter : IDataImporter
    {
        public BankDataSnapshot Import(string path)
        {
            var data = ReadFile(path);
            /*
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
            */

            var snapshot = Parse(data);

            Validate(snapshot);

            return snapshot;
        }
        public abstract BankDataSnapshot Parse(string content);
        protected string ReadFile(string path) => File.ReadAllText(path);
        public virtual void Validate(BankDataSnapshot snapshot)
        {
            if (snapshot is null)
                throw new InvalidOperationException("Файл прочитан, но snapshot == null");

            if (snapshot.Accounts is null || snapshot.Categories is null)
                throw new InvalidOperationException("В файле отсутствуют разделы Accounts или Categories");

            if (snapshot.Accounts.Any(a => a is null))
                throw new InvalidOperationException("В списке Accounts обнаружены null-элементы");

            if (snapshot.Categories.Any(c => c is null))
                throw new InvalidOperationException("В списке Categories обнаружены null-элементы");

            var accountIds = new HashSet<Guid>();
            foreach (var acc in snapshot.Accounts)
            {
                if (!accountIds.Add(acc.AccountId))
                    throw new InvalidOperationException($"Дублирующийся AccountId: {acc.AccountId}");
                if (acc.Operations is null)
                    throw new InvalidOperationException($"У аккаунта {acc.AccountId} коллекция Operations == null");
            }

            var categoryIds = new HashSet<Guid>();
            foreach (var cat in snapshot.Categories)
            {
                if (!categoryIds.Add(cat.CategoryId))
                    throw new InvalidOperationException($"Дублирующийся CategoryId: {cat.CategoryId}");

                if (string.IsNullOrWhiteSpace(cat.Name))
                    throw new InvalidOperationException($"Пустое имя у категории {cat.CategoryId}");
            }
            var catNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var cat in snapshot.Categories)
            {
                var name = cat.Name.Trim();
                if (!catNames.Add(name))
                    throw new InvalidOperationException($"Дублирующееся имя категории (без учёта регистра): \"{name}\"");
            }
            var globalOpIds = new HashSet<Guid>();
            var now = DateTime.Now;

            foreach (var acc in snapshot.Accounts)
            {
                foreach (var op in acc.Operations)
                {
                    if (op is null)
                        throw new InvalidOperationException($"В аккаунте {acc.AccountId} обнаружена операция == null");

                    if (op.AccountId != acc.AccountId)
                        throw new InvalidOperationException(
                            $"Операция {op.OperationId} лежит в аккаунте {acc.AccountId}, но ссылается на AccountId = {op.AccountId}.");

                    if (!categoryIds.Contains(op.CategoryId))
                        throw new InvalidOperationException(
                            $"Операция {op.OperationId} ссылается на неизвестную категорию: {op.CategoryId}");

                    if (!globalOpIds.Add(op.OperationId))
                        throw new InvalidOperationException($"Дублирующийся OperationId: {op.OperationId}");

                    if (op.Amount <= 0)
                        throw new InvalidOperationException(
                            $"Операция {op.OperationId}: сумма должна быть > 0. Сейчас: {op.Amount}");

                    if (op.Date > now)
                        throw new InvalidOperationException(
                            $"Операция {op.OperationId}: дата в будущем не допускается. Сейчас: {op.Date:yyyy-MM-dd HH:mm:ss}");
                }
            }
        }
    }
}