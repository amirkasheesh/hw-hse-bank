namespace CategorySpace
{
    public class Category
    {
        public required Guid CategoryId { get; set; }
        public required OperationType Type { get; set; }
        public required string Name { get; set; }
    }
}