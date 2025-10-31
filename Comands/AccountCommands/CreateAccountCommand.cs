using Facades;
using Commands;

namespace AccountCommands
{
    public class CreateAccountCommand : ICommand
    {
        private AccountsFacade _accountsFacade;
        public CreateAccountCommand(AccountsFacade accountsFacade)
        {
            _accountsFacade = accountsFacade;
        }

        public string Name => "Создать счет;";

        public void Execute()
        {
            _accountsFacade.CreateAccount();
        }
    }
}