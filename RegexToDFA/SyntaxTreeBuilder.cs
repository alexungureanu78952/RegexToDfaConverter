public class SyntaxTreeBuilder
{
    public TreeNode BuildFromPostfix(string postfix)
    {
        var stack = new Stack<TreeNode>();

        foreach (char c in postfix)
        {
            if (char.IsLetterOrDigit(c))
            {
                stack.Push(new TreeNode(c.ToString()));
            }
            else if (c == '|' || c == '.')
            {
                var right = stack.Pop();
                var left = stack.Pop();
                var node = new TreeNode(c.ToString())
                {
                    Left = left,
                    Right = right
                };
                stack.Push(node);
            }
            else if (c == '*')
            {
                var child = stack.Pop();
                var node = new TreeNode("*")
                {
                    Left = child
                };
                stack.Push(node);
            }
        }

        return stack.Count > 0 ? stack.Pop() : null;
    }

    public void PrintTree(TreeNode node, string indent = "", bool last = true)
    {
        if (node == null) return;

        Console.Write(indent);
        if (last)
        {
            Console.Write("└─");
            indent += "  ";
        }
        else
        {
            Console.Write("├─");
            indent += "│ ";
        }

        Console.WriteLine(node.Value);

        if (node.Left != null && node.Right != null)
        {
            PrintTree(node.Left, indent, false);
            PrintTree(node.Right, indent, true);
        }
        else if (node.Left != null)
        {
            PrintTree(node.Left, indent, true);
        }
    }
}

