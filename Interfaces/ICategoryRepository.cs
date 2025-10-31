using CategorySpace;

namespace Interfaces
{
    public interface ICategoryRepository
    {
        public List<Category> GetAllCategories();
        public Category? FindCategoryByName(string? Name);
        public Category? FindCategoryById(Guid id);
        public void AddNewCategory(Category category);
        public void Clear();
        public int ClearCategory(Guid id);
    }
}