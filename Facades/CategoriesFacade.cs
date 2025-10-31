using Interfaces;
using CategorySpace;

namespace Facades
{
    public class CategoriesFacade
    {
        private ICategoryRepository _categoryRepository;
        private IAccountRepository _accountRepository;
        public CategoriesFacade(ICategoryRepository categoryRepository, IAccountRepository accountRepository)
        {
            _categoryRepository = categoryRepository;
            _accountRepository = accountRepository;
        }
        public void AddSomeCategory()
        {
            Console.Clear();
            System.Console.WriteLine("Давайте добавим новую категорию!\n");

            System.Console.WriteLine("Список доступных:");
            var categories = _categoryRepository.GetAllCategories();
            for (int i = 0; i < categories.Count; ++i)
            {
                System.Console.WriteLine($"\nКатегория №{i + 1}");
                System.Console.WriteLine($"Название: {categories[i].Name}");
                System.Console.WriteLine($"Тип: {categories[i].Type}");
            }

            string name;
            while (true)
            {
                System.Console.Write("\nВведите название категории (0 - назад): ");
                string? cmd = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(cmd))
                {
                    System.Console.WriteLine("\nВы ничего не ввели! Попробуйте еще раз!");
                }
                else if (cmd.Trim() == "0")
                {
                    return;
                }
                else if (_categoryRepository.FindCategoryByName(cmd) != null)
                {
                    System.Console.WriteLine("Категория с таким именем уже есть! Попробуйте еще раз!");
                }
                else
                {
                    System.Console.WriteLine("Название записано!");
                    name = cmd;
                    break;
                }
            }

            OperationType type = new OperationType();
            while (true)
            {
                System.Console.Write("\nВведите тип категории (расход/доход) (0 - назад): ");
                string? cmd1 = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(cmd1))
                {
                    System.Console.WriteLine("\nВы ничего не ввели! Попробуйте еще раз!");
                }
                else if (cmd1.Trim() == "0")
                {
                    return;
                }
                else if (cmd1.Trim().ToLower() == "расход")
                {
                    type = OperationType.Expense;
                    break;
                }
                else if (cmd1.Trim().ToLower() == "доход")
                {
                    type = OperationType.Income;
                    break;
                }
                else
                {
                    System.Console.WriteLine("\nНужно четко ввести по буквам расход ИЛИ доход! Попробуйте еще раз!");
                }
            }

            Guid guid = Guid.NewGuid();
            Category category = new Category { CategoryId = guid, Type = type, Name = name };

            _categoryRepository.AddNewCategory(category);
            System.Console.WriteLine("\nКатегория была добавлена!");
            return;
        }

        public void DeleteOneCategory()
        {
            Console.Clear();
            System.Console.WriteLine("Давайте удалим категорию!");

            System.Console.WriteLine("Список доступных:");

            var categories = _categoryRepository.GetAllCategories();
            for (int i = 0; i < categories.Count; ++i)
            {
                System.Console.WriteLine($"\nКатегория №{i + 1}");
                System.Console.WriteLine($"Название: {categories[i].Name};");
                System.Console.WriteLine($"Id категории: {categories[i].CategoryId};");
                System.Console.WriteLine($"Тип: {categories[i].Type}.");
            }

            while (true)
            {
                System.Console.Write("\nВведите(скопируйте) ID категории (0 - назад): ");
                var cmd_id_inside = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(cmd_id_inside))
                {
                    System.Console.WriteLine("Вы ничего не ввели! Попробуйте еще раз!");
                }
                else if (cmd_id_inside.Trim() == "0")
                {
                    return;
                }
                else if (!Guid.TryParse(cmd_id_inside.Trim(), out var id))
                {
                    System.Console.WriteLine("Неверный формат ID (возможно, произошло неточное копирование). Попробуйте еще раз");
                }
                else
                {
                    if (_accountRepository.CheckIfExistsCategoryById(id))
                    {
                        System.Console.WriteLine("\nНельзя удалить категорию, которая содержится в операциях! Выберите другую!");
                        continue;
                    }
                    if (_categoryRepository.ClearCategory(id) == 1)
                    {
                        System.Console.WriteLine("\nКатегория была удалена!");
                        return;
                    }
                    System.Console.WriteLine("Категории с таким ID нет! Попробуйте еще раз!");
                }
            }
        }
    }
}