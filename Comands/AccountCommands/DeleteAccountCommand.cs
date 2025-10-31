using Facades;
using Commands;

namespace AccountCommands
{
    public class DeleteAccountCommand : ICommand
    {
        private AccountsFacade _accountsFacade;
        public DeleteAccountCommand(AccountsFacade accountsFacade)
        {
            _accountsFacade = accountsFacade;
        }

        public string Name => "Удалить счет;";

        public void Execute()
        {
            _accountsFacade.DeleteOneBankAccount();
        }
    }
}