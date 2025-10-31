using Facades;
using Commands;

namespace AnalyticsCommands
{
    public class CheckBalanceCommand : ICommand
    {
        private AnalyticsFacade _analyticsFacade;
        public CheckBalanceCommand(AnalyticsFacade analyticsFacade)
        {
            _analyticsFacade = analyticsFacade;
        }

        public string Name => "Самопроверка баланса. Пересчитать из операций;";

        public void Execute()
        {
            _analyticsFacade.CheckYourBalanceFunc();
        }
    }
}