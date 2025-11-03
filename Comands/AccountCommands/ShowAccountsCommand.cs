using Facades;
using Commands;

namespace AccountCommands
{
    public class ShowAccountsCommand : ICommand
    {
        private AccountsFacade _accountsFacade;
        public ShowAccountsCommand(AccountsFacade accountsFacade)
        {
            _accountsFacade = accountsFacade;
        }

        public string Name => "Показать счета;";

        public void Execute()
        {
            _accountsFacade.ShowAccounts();
        }
    }
}