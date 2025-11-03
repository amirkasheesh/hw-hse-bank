using System.Diagnostics;
using Interfaces;
using BankAccountSpace;

namespace Infrastructure
{
    public sealed class LoggingAccountRepository : IAccountRepository
    {
        private readonly IAccountRepository _inner;

        public LoggingAccountRepository(IAccountRepository inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public void AddNewAccount(BankAccount? account)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Console.WriteLine($"[Репозиторий/Счета] Добавление счёта: Id = {account?.AccountId}, Имя = {account?.Name}");
                _inner.AddNewAccount(account);
                Console.WriteLine($"[Репозиторий/Счета] Добавление счёта: ОК ({sw.ElapsedMilliseconds} мс)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Репозиторий/Счета] Добавление счёта: ОШИБКА → {ex.Message}");
                throw;
            }
        }

        public bool CheckIfExistsByName(string? name)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Console.WriteLine($"[Репозиторий/Счета] Проверка наличия по имени: \"{name}\"");
                var result = _inner.CheckIfExistsByName(name);
                Console.WriteLine($"[Репозиторий/Счета] Проверка наличия по имени: {result} ({sw.ElapsedMilliseconds} мс)");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Репозиторий/Счета] Проверка наличия по имени: ОШИБКА → {ex.Message}");
                throw;
            }
        }

        public bool CheckIfExistsCategoryById(Guid id)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Console.WriteLine($"[Репозиторий/Счета] Проверка использования категории: CategoryId = {id}");
                var result = _inner.CheckIfExistsCategoryById(id);
                Console.WriteLine($"[Репозиторий/Счета] Проверка использования категории: {result} ({sw.ElapsedMilliseconds} мс)");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Репозиторий/Счета] Проверка использования категории: ОШИБКА → {ex.Message}");
                throw;
            }
        }

        public void Clear()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Console.WriteLine("[Репозиторий/Счета] Очистка всех счетов");
                _inner.Clear();
                Console.WriteLine($"[Репозиторий/Счета] Очистка всех счетов: ОК ({sw.ElapsedMilliseconds} мс)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Репозиторий/Счета] Очистка всех счетов: ОШИБКА → {ex.Message}");
                throw;
            }
        }

        public void Clear(Guid id)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Console.WriteLine($"[Репозиторий/Счета] Удаление счёта: Id = {id}");
                _inner.Clear(id);
                Console.WriteLine($"[Репозиторий/Счета] Удаление счёта: ОК ({sw.ElapsedMilliseconds} мс)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Репозиторий/Счета] Удаление счёта: ОШИБКА → {ex.Message}");
                throw;
            }
        }

        public BankAccount? FindAccountByID(Guid id)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Console.WriteLine($"[Репозиторий/Счета] Поиск счёта по Id: {id}");
                var acc = _inner.FindAccountByID(id);
                Console.WriteLine($"[Репозиторий/Счета] Поиск счёта: {(acc is null ? "не найден" : "найден")} ({sw.ElapsedMilliseconds} мс)");
                return acc;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Репозиторий/Счета] Поиск счёта: ОШИБКА → {ex.Message}");
                throw;
            }
        }

        public List<BankAccount> GetAllAccounts()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Console.WriteLine("[Репозиторий/Счета] Получение всех счетов");
                var list = _inner.GetAllAccounts();
                Console.WriteLine($"[Репозиторий/Счета] Получение всех счетов: {list.Count} шт. ({sw.ElapsedMilliseconds} мс)");
                return list.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Репозиторий/Счета] Получение всех счетов: ОШИБКА → {ex.Message}");
                throw;
            }
        }

        public void ShowAllAccounts()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Console.WriteLine("[Репозиторий/Счета] Печать всех счетов");
                _inner.ShowAllAccounts();
                Console.WriteLine($"[Репозиторий/Счета] Печать всех счетов: ОК ({sw.ElapsedMilliseconds} мс)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Репозиторий/Счета] Печать всех счетов: ОШИБКА → {ex.Message}");
                throw;
            }
        }
    }
}
