using System.Collections.Generic;

internal class QueryModel
{
    public string TargetTable { get; set; }
    public IEnumerable<string> TargetColumns { get; set; }
}