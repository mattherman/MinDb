namespace MinDb.Compiler.Models
{
    internal class DeleteQueryModel : QueryModel
    {
        public ConditionModel Condition { get; set; }

        public override string ToString()
        {
            return $"DELETE | Table = {TargetTable.Name}, Condition = {Condition}";
        }
    }
}