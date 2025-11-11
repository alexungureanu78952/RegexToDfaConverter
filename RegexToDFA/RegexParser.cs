using System.Text;

public class RegexParser
{
    public string AddExplicitConcatenation(string regex)
    {
        var result = new StringBuilder();

        for (int i = 0; i < regex.Length; i++)
        {
            char current = regex[i];
            result.Append(current);

            if (i < regex.Length - 1)
            {
                char next = regex[i + 1];

                bool currentIsOperand = char.IsLetterOrDigit(current);
                bool currentIsCloseParen = current == ')';
                bool currentIsStar = current == '*';
                bool nextIsOperand = char.IsLetterOrDigit(next);
                bool nextIsOpenParen = next == '(';

                if ((currentIsOperand || currentIsCloseParen || currentIsStar) &&
                    (nextIsOperand || nextIsOpenParen))
                {
                    result.Append('.');
                }
            }
        }

        return result.ToString();
    }

    public string ConvertToPostfix(string regex)
    {
        var output = new StringBuilder();
        var operatorStack = new Stack<char>();

        var precedence = new Dictionary<char, int>
        {
            { '|', 1 },
            { '.', 2 },
            { '*', 3 }
        };

        foreach (char c in regex)
        {
            if (char.IsLetterOrDigit(c))
            {
                output.Append(c);
            }
            else if (c == '(')
            {
                operatorStack.Push(c);
            }
            else if (c == ')')
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
                {
                    output.Append(operatorStack.Pop());
                }
                if (operatorStack.Count > 0)
                {
                    operatorStack.Pop();
                }
            }
            else if (precedence.ContainsKey(c))
            {
                while (operatorStack.Count > 0 &&
                       operatorStack.Peek() != '(' &&
                       precedence.ContainsKey(operatorStack.Peek()) &&
                       precedence[operatorStack.Peek()] >= precedence[c])
                {
                    output.Append(operatorStack.Pop());
                }
                operatorStack.Push(c);
            }
        }

        while (operatorStack.Count > 0)
        {
            output.Append(operatorStack.Pop());
        }

        return output.ToString();
    }
}

