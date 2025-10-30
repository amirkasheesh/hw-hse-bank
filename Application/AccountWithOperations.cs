using OperationSpace;

namespace Application
{
    public class AccountWithOperations
    {
        public Guid AccountId { get; set; }
        public string Name { get; set; } = "";
        public decimal Balance { get; set; }
        public List<Operation> Operations { get; set; } = new();
    }
}