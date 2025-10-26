namespace Category
{
    public enum OperationType
    {
        Expense,
        Income
    }
    class Category
    {
        public required Guid IdCategory { get; set; }
        public required OperationType Type { get; set; }
        public required string Name { get; set; }
    }
}