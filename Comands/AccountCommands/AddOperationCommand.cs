using Facades;
using Commands;

namespace AccountCommands
{
    public class AddOperationCommand : ICommand
    {
        private AccountsFacade _accountsFacade;
        public AddOperationCommand(AccountsFacade accountsFacade)
        {
            _accountsFacade = accountsFacade;
        }

        public string Name => "Добавить операцию;";

        public void Execute()
        {
            _accountsFacade.AddSomeOperation();
        }
    }
}