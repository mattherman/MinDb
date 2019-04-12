namespace MinDb.Compiler.Models
{
    internal class DeleteQueryModel : QueryModel
    {
        public override string ToString()
        {
            return $"DELETE | Table = {TargetTable}";
        }
    }
}