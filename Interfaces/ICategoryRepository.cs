using CategorySpace;

namespace Interfaces
{
    public interface ICategoryRepository
    {
        public List<Category> GetAllCategories();
        public Category? FindCategoryByName(string? Name);
        public Category? FindCategoryById(Guid id);
    }
}