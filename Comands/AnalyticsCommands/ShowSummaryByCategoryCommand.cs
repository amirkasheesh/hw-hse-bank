using Facades;
using Commands;

namespace AnalyticsCommands
{
    public class ShowSummaryByCategoryCommand : ICommand
    {
        private AnalyticsFacade _analyticsFacade;
        public ShowSummaryByCategoryCommand(AnalyticsFacade analyticsFacade)
        {
            _analyticsFacade = analyticsFacade;
        }

        public string Name => "Сводка по категориям за период;";

        public void Execute()
        {
            _analyticsFacade.GiveSummaryByCategoryForPeriod();
        }
    }
}