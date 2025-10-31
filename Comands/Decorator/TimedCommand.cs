using Commands;

namespace Decorator
{
    public class TimedCommand : ICommand
    {
        private ICommand _command;
        public TimedCommand(ICommand command)
        {
            _command = command;
        }
        public string Name => _command.Name;

        public void Execute()
        {
            DateTime start = DateTime.Now;
            _command.Execute();
            System.Console.WriteLine($"\nОперация длилась: {(DateTime.Now - start).ToString(@"hh\:mm\:ss\.fff")}");
        }
    }
}