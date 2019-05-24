namespace MinDb.Compiler.Models
{
    internal class ConditionModel : BinaryTree<IConditionNode>
    {

    }

    internal interface IConditionNode { }

    internal class BinaryTree<T>
    {
        public BinaryTree<T> Left { get; set; }
        public T Value { get; set; }
        public BinaryTree<T> Right { get; set; }

        public override string ToString() => 
            Left == null && Right == null ?
                Value.ToString() :
                $"[{Value} {Left} {Right}]";
    }
}