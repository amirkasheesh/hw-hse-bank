using Category;

namespace Operation
{
    class Operation
    {
        public required Guid IdOperation { get; set; }
        public required OperationType Type { get; set; }
        public required Guid AccountId { get; set; }
        public required int Amount { get; set; }
        public required DateTime Date { get; set; }
        public required Guid CategoryId { get; set; }
    }
}