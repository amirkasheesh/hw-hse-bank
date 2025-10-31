using Facades;
using Commands;

namespace CategoryCommands
{
    public class DeleteCategoryCommand : ICommand
    {
        private CategoriesFacade _categoriesFacade;
        public DeleteCategoryCommand(CategoriesFacade categoriesFacade)
        {
            _categoriesFacade = categoriesFacade;
        }

        public string Name => "Удалить категорию;";

        public void Execute()
        {
            _categoriesFacade.DeleteOneCategory();
        }
    }
}