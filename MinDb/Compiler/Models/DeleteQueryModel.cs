internal class DeleteQueryModel : QueryModel
{
    public override string ToString()
    {
        return $"DELETE | Table = {TargetTable}";
    } 
}