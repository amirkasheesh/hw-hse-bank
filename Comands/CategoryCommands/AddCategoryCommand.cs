using Facades;
using Commands;

namespace CategoryCommands
{
    public class AddSomeCategoryCommand : ICommand
    {
        private CategoriesFacade _categoriesFacade;
        public AddSomeCategoryCommand(CategoriesFacade categoriesFacade)
        {
            _categoriesFacade = categoriesFacade;
        }

        public string Name => "Добавить/посмотреть категорию;";

        public void Execute()
        {
            _categoriesFacade.AddSomeCategory();
        }
    }
}