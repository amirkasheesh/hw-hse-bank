namespace BankAccount
{
    class BankAccount
    {
        public required Guid IdAccount { get; set; }
        public required string Name { get; set; }
        public required int Balance { get; set; }
    }
}