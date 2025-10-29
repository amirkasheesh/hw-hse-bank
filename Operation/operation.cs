namespace OperationSpace
{
    public class Operation
    {
        public Guid OperationId { get; set; }
        public OperationType Type { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Guid CategoryId { get; set; }
        public string? Description { get; set; }

        public Operation(Guid operationId, OperationType type, decimal amount, DateTime date, Guid categoryId)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Сумма не может быть отрицательной или ноль");
            }
            if (operationId == Guid.Empty || categoryId == Guid.Empty)
            {
                throw new ArgumentException("Айди должны быть валидными!");
            }
            OperationId = operationId;
            Type = type;
            Amount = amount;
            Date = date;
            CategoryId = categoryId;
        }
        public Operation(Guid operationId, OperationType type, decimal amount, DateTime date, Guid categoryId, string description)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Сумма не может быть отрицательной или ноль");
            }
            if (operationId == Guid.Empty || categoryId == Guid.Empty)
            {
                throw new ArgumentException("Айди должны быть валидными!");
            }
            OperationId = operationId;
            Type = type;
            Amount = amount;
            Date = date;
            CategoryId = categoryId;
            Description = description;
        }
    }
}