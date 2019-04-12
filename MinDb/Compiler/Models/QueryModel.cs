using System.Collections.Generic;

namespace MinDb.Compiler.Models
{
    internal abstract class QueryModel
    {
        public ObjectModel TargetTable { get; set; }
    }
}