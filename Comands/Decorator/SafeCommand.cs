using System.Runtime.ConstrainedExecution;
using Commands;

namespace Decorator
{
    public class SafeCommand : ICommand
    {
        private ICommand _command;
        public SafeCommand(ICommand command)
        {
            _command = command;
        }
        public string Name => _command.Name;

        public void Execute()
        {
            try
            {
                _command.Execute();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Произошла ошибка: {ex.Message}");
                return;
            }
        }
    }
}