using Facades;

namespace Commands
{
    public interface ICommand
    {
        public void Execute();
        string Name { get; }
    }
}