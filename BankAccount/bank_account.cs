using CategorySpace;
using Microsoft.VisualBasic;
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

        public List<Operation> ShowOperationsByDate(DateTime from, DateTime to)
        {
            var operations = _collectionOfOperations.Where(op => op.Date >= from && op.Date <= to).ToList();
            operations.Sort((op_1, op_2) => op_1.Date.CompareTo(op_2.Date));
            return operations;
        }

        public (decimal income, decimal expense, decimal total) CalculateTotalsByDate(DateTime from, DateTime to)
        {
            var sorted_operations = ShowOperationsByDate(from, to);

            decimal sum_of_expense = 0;
            decimal sum_of_income = 0;
            foreach (var el in sorted_operations)
            {
                if (el.Type == OperationType.Expense)
                {
                    sum_of_expense += el.Amount;
                }
                else
                {
                    sum_of_income += el.Amount;
                }
            }

            decimal res = sum_of_income - sum_of_expense;
            return (sum_of_income, sum_of_expense, res);
        }
    }
}