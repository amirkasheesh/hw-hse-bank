namespace Application
{
    public interface IDataImporter
    {
        public BankDataSnapshot Import(string path);
    }
}