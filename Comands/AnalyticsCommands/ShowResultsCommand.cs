using Facades;
using Commands;

namespace AnalyticsCommands
{
    public class ShowResultsCommand : ICommand
    {
        private AnalyticsFacade _analyticsFacade;
        public ShowResultsCommand(AnalyticsFacade analyticsFacade)
        {
            _analyticsFacade = analyticsFacade;
        }

        public string Name => "Итоги (доход/расход/итог) за период;";

        public void Execute()
        {
            _analyticsFacade.ResultsByPeriod();
        }
    }
}