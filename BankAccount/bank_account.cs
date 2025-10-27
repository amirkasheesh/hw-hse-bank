using CategorySpace;
using OperationSpace;

namespace BankAccountSpace
{
    public class BankAccount
    {
        public required Guid AccountId { get; set; }
        public required string Name { get; set; }
        private decimal _balance = 0;
        public decimal Balance
        {
            get
            {
                return _balance;
            }
            set
            {
                _balance = value;
            }
        }
        private List<Operation> _collectionOfOperations = new List<Operation>();
        public void AddOperation(decimal amount, DateTime date, Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException("Передан недействительный объект!");
            }

            if (this.AccountId == Guid.Empty || category.CategoryId == Guid.Empty)
            {
                throw new ArgumentException("Айди должны быть валидными!");
            }

            if (date == DateTime.MinValue || date == DateTime.MaxValue)
            {
                throw new InvalidOperationException("Дата не установлена должным образом!");
            }

            Guid guid = Guid.NewGuid();
            Operation local_operation = new Operation(guid, category.Type, amount, date, category.CategoryId);
            local_operation.AccountId = this.AccountId;

            if (local_operation.Type == OperationType.Income)
            {
                Balance += local_operation.Amount;
            }
            else
            {
                if (Balance >= local_operation.Amount)
                {
                    Balance -= local_operation.Amount;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Баланс не может быть отрицательным!");
                }
            }
            _collectionOfOperations.Add(local_operation);
        }
        public List<Operation> ShowOperations()
        {
            return _collectionOfOperations.ToList();
        }
    }
}