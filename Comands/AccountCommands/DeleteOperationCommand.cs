using Facades;
using Commands;

namespace AccountCommands
{
    public class DeleteOperationCommand : ICommand
    {
        private AccountsFacade _accountsFacade;
        public DeleteOperationCommand(AccountsFacade accountsFacade)
        {
            _accountsFacade = accountsFacade;
        }

        public string Name => "Удалить операцию у счета;";

        public void Execute()
        {
            _accountsFacade.DeleteOperationInAccount();
        }
    }
}