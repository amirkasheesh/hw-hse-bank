namespace Application
{
    public interface IDataExporter
    {
        public void Export(BankDataSnapshot snapshot, string path);
    }
}