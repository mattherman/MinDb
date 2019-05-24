namespace MinDb.Compiler.Models
{
    internal class ObjectModel : IConditionNode
    {
        public string Name { get; }

        public ObjectModel(string Name)
        {
            this.Name = Name;
        }

        public override string ToString() => Name;
    }
}