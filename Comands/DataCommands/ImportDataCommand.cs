using Facades;
using Commands;

namespace DataCommands
{
    public class ImportDataCommand : ICommand
    {
        private ImportExportFacade _dataFacade;
        public ImportDataCommand(ImportExportFacade dataFacade)
        {
            _dataFacade = dataFacade;
        }
        public string Name => "Импорт данных;";

        public void Execute()
        {
            _dataFacade.ImportData();
        }
    }
}