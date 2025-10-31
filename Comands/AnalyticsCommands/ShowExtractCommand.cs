using Facades;
using Commands;

namespace AnalyticsCommands
{
    public class ShowExtractCommand : ICommand
    {
        private AnalyticsFacade _analyticsFacade;
        public ShowExtractCommand(AnalyticsFacade analyticsFacade)
        {
            _analyticsFacade = analyticsFacade;
        }

        public string Name => "Выписка за период;";

        public void Execute()
        {
            _analyticsFacade.GiveExtractByPeriod();
        }
    }
}