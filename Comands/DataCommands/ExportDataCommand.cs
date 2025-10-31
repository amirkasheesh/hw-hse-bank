using Facades;
using Commands;

namespace DataCommands
{
    public class ExportDataCommand : ICommand
    {
        private ImportExportFacade _dataFacade;
        public ExportDataCommand(ImportExportFacade dataFacade)
        {
            _dataFacade = dataFacade;
        }

        public string Name => "Экспорт данных;";

        public void Execute()
        {
            _dataFacade.ExportData();
        }
    }
}