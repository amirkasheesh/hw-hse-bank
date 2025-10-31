using CategorySpace;

namespace Application
{
    public class BankDataSnapshot
    {
        public List<Category> Categories { get; set; } = new();
        public List<AccountWithOperations> Accounts { get; set; } = new();
    }
}