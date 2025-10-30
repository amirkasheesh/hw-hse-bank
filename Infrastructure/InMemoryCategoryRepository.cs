using CategorySpace;
using Interfaces;

namespace Infrastructure
{
    public class InMemoryCategoryRepository : ICategoryRepository
    {
        private readonly List<Category> _categories = new List<Category>
        {
            new Category {CategoryId = Guid.NewGuid(), Name = "Зарплата", Type = OperationType.Income },
            new Category {CategoryId = Guid.NewGuid(), Name = "Кэшбек", Type = OperationType.Income},

            new Category {CategoryId = Guid.NewGuid(), Name = "Рестораны/Кафе", Type = OperationType.Expense},
            new Category {CategoryId = Guid.NewGuid(), Name = "Транспорт", Type = OperationType.Expense},
            new Category {CategoryId = Guid.NewGuid(), Name = "Здоровье", Type = OperationType.Expense},
            new Category {CategoryId = Guid.NewGuid(), Name = "Развлечения", Type = OperationType.Expense}
        };

        public Category? FindCategoryById(Guid id)
        {
            foreach (var el in _categories)
            {
                if (el.CategoryId == id)
                {
                    return el;
                }
            }
            return null;
            
        }

        public Category? FindCategoryByName(string? name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("Вместо названия был передан null-обьект!");
            }

            foreach (var el in _categories)
            {
                if (el.Name.ToLower() == name.Trim().ToLower())
                {
                    return el;
                }
            }
            return null;
        }

        public List<Category> GetAllCategories()
        {
            return _categories.ToList();
        }
    }
}