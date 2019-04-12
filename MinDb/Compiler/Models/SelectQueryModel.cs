using System.Collections.Generic;

internal class SelectQueryModel : QueryModel
{
    public IEnumerable<string> TargetColumns { get; set; }

    public override string ToString()
    {
        return $"SELECT | Table = {TargetTable}, Columns = [{string.Join(",", TargetColumns)}]";
    }
}