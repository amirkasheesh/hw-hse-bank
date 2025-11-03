using System.Diagnostics;
using Interfaces;
using CategorySpace;

namespace Infrastructure
{
    public sealed class LoggingCategoryRepository : ICategoryRepository
    {
        private readonly ICategoryRepository _inner;

        public LoggingCategoryRepository(ICategoryRepository inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public void AddNewCategory(Category category)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Console.WriteLine($"[Репозиторий/Категории] Добавление категории: Id = {category.CategoryId}, Имя = {category.Name}, Тип = {category.Type}");
                _inner.AddNewCategory(category);
                Console.WriteLine($"[Репозиторий/Категории] Добавление категории: ОК ({sw.ElapsedMilliseconds} мс)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Репозиторий/Категории] Добавление категории: ОШИБКА → {ex.Message}");
                throw;
            }
        }

        public void Clear()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Console.WriteLine("[Репозиторий/Категории] Очистка всех категорий");
                _inner.Clear();
                Console.WriteLine($"[Репозиторий/Категории] Очистка всех категорий: ОК ({sw.ElapsedMilliseconds} мс)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Репозиторий/Категории] Очистка всех категорий: ОШИБКА → {ex.Message}");
                throw;
            }
        }

        public int ClearCategory(Guid id)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Console.WriteLine($"[Репозиторий/Категории] Удаление категории: Id={id}");
                var removed = _inner.ClearCategory(id);
                Console.WriteLine($"[Репозиторий/Категории] Удаление категории: удалено = {removed} ({sw.ElapsedMilliseconds} мс)");
                return removed;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Репозиторий/Категории] Удаление категории: ОШИБКА → {ex.Message}");
                throw;
            }
        }

        public Category? FindCategoryById(Guid id)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Console.WriteLine($"[Репозиторий/Категории] Поиск категории по Id: {id}");
                var cat = _inner.FindCategoryById(id);
                Console.WriteLine($"[Репозиторий/Категории] Поиск категории: {(cat is null ? "не найдена" : "найдена")} ({sw.ElapsedMilliseconds} мс)");
                return cat;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Репозиторий/Категории] Поиск категории: ОШИБКА → {ex.Message}");
                throw;
            }
        }

        public Category? FindCategoryByName(string? name)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Console.WriteLine($"[Репозиторий/Категории] Поиск категории по имени: \"{name}\"");
                var cat = _inner.FindCategoryByName(name);
                Console.WriteLine($"[Репозиторий/Категории] Поиск категории по имени: {(cat is null ? "не найдена" : "найдена")} ({sw.ElapsedMilliseconds} мс)");
                return cat;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Репозиторий/Категории] Поиск категории по имени: ОШИБКА → {ex.Message}");
                throw;
            }
        }

        public List<Category> GetAllCategories()
        {
            var sw = Stopwatch.StartNew();
            try
            {
                Console.WriteLine("[Репозиторий/Категории] Получение всех категорий");
                var list = _inner.GetAllCategories();
                Console.WriteLine($"[Репозиторий/Категории] Получение всех категорий: {list.Count} шт. ({sw.ElapsedMilliseconds} мс)");
                return list.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Репозиторий/Категории] Получение всех категорий: ОШИБКА → {ex.Message}");
                throw;
            }
        }
    }
}
