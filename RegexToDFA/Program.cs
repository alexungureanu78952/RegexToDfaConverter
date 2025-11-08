using System;
using System.IO;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        
        // Citește expresia regulată din fișier
        string regex = ReadRegexFromFile("regex.txt");
        if (string.IsNullOrEmpty(regex))
        {
            Console.WriteLine("Eroare: Fișierul regex.txt nu există sau este gol.");
            return;
        }

        Console.WriteLine($"Expresie regulată citită: {regex}");
        Console.WriteLine();

        // Construiește automatul finit determinist
        var converter = new RegexToDfaConverter();
        DeterministicFiniteAutomaton dfa = null;
        string postfixForm = "";
        
        try
        {
            // Construiește DFA
            dfa = converter.RegexToDFA(regex);
            
            // Obține forma postfixată (refacem pașii pentru a o obține)
            string explicitRegex = AddExplicitConcatenation(regex);
            postfixForm = ConvertToPostfix(explicitRegex);
            
            Console.WriteLine("Automatul finit determinist a fost construit cu succes!");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la construirea automatului: {ex.Message}");
            return;
        }

        // Meniu interactiv
        bool running = true;
        while (running)
        {
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.WriteLine("              MENIU PRINCIPAL");
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.WriteLine("1. Afișare formă polonează postfixată");
            Console.WriteLine("2. Afișare arbore sintactic");
            Console.WriteLine("3. Afișare automat în consolă");
            Console.WriteLine("4. Salvare automat în fișier");
            Console.WriteLine("5. Verificare cuvânt");
            Console.WriteLine("6. Verificare multiple cuvinte");
            Console.WriteLine("0. Ieșire");
            Console.WriteLine("═══════════════════════════════════════════════════");
            Console.Write("Alegeți o opțiune: ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    DisplayPostfixForm(postfixForm);
                    break;
                case "2":
                    DisplaySyntaxTree(postfixForm);
                    break;
                case "3":
                    DisplayAutomaton(dfa);
                    break;
                case "4":
                    SaveAutomatonToFile(dfa);
                    break;
                case "5":
                    CheckSingleWord(dfa);
                    break;
                case "6":
                    CheckMultipleWords(dfa);
                    break;
                case "0":
                    running = false;
                    Console.WriteLine("La revedere!");
                    break;
                default:
                    Console.WriteLine("Opțiune invalidă. Încercați din nou.");
                    break;
            }
            
            if (running)
            {
                Console.WriteLine("\nApăsați orice tastă pentru a continua...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    static string ReadRegexFromFile(string filename)
    {
        try
        {
            // Lista de locații unde să căutăm fișierul
            string[] possiblePaths = new string[]
            {
                filename,  // În directorul curent
                Path.Combine("..", filename),  // Un nivel mai sus
                Path.Combine("..", "..", filename),  // Două nivele mai sus
                Path.Combine("..", "..", "..", filename),  // Trei nivele mai sus
                Path.Combine("RegexToDFA", filename),  // În subdirectorul RegexToDFA
                Path.Combine("bin", "Debug", "net9.0", filename),  // În directorul de build
                Path.Combine(@"D:\cod\LFC\RegexToDFA", filename),  // Calea absolută
                Path.Combine(@"D:\cod\LFC\RegexToDFA\RegexToDFA", filename)  // Calea absolută în subdirector
            };

            Console.WriteLine($"Căutare fișier '{filename}'...");
            Console.WriteLine($"Directorul curent: {Directory.GetCurrentDirectory()}");
            Console.WriteLine();

            foreach (string path in possiblePaths)
            {
                string fullPath = Path.GetFullPath(path);
                Console.WriteLine($"Verific: {fullPath}");
                
                if (File.Exists(fullPath))
                {
                    string content = File.ReadAllText(fullPath).Trim();
                    Console.WriteLine($"✓ Fișier găsit!");
                    Console.WriteLine();
                    return content;
                }
            }

            Console.WriteLine();
            Console.WriteLine($"✗ Fișierul '{filename}' nu a fost găsit în nicio locație.");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la citirea fișierului: {ex.Message}");
        }
        return null;
    }

    static void DisplayPostfixForm(string postfixForm)
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════╗");
        Console.WriteLine("║        FORMA POLONEAZĂ POSTFIXATĂ                ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine($"Forma postfixată: {postfixForm}");
        Console.WriteLine();
        
        // Afișare pas cu pas
        Console.WriteLine("Explicație:");
        Console.WriteLine("  '|' - alternare (OR)");
        Console.WriteLine("  '.' - concatenare");
        Console.WriteLine("  '*' - Kleene star (repetare)");
        Console.WriteLine();
    }

    static void DisplaySyntaxTree(string postfixForm)
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════╗");
        Console.WriteLine("║           ARBORE SINTACTIC                        ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════╝");
        Console.WriteLine();
        
        var tree = BuildSyntaxTree(postfixForm);
        PrintTree(tree, "", true);
        Console.WriteLine();
    }

    static void DisplayAutomaton(DeterministicFiniteAutomaton dfa)
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════╗");
        Console.WriteLine("║     AUTOMATUL FINIT DETERMINIST                   ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════╝");
        Console.WriteLine();
        
        if (dfa.VerifyAutomaton())
        {
            Console.WriteLine("✓ Automatul este valid");
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("✗ ATENȚIE: Automatul nu este valid!");
            Console.WriteLine();
        }
        
        Console.WriteLine(dfa.PrintAutomaton());
    }

    static void SaveAutomatonToFile(DeterministicFiniteAutomaton dfa)
    {
        Console.Write("Introduceți numele fișierului de ieșire (implicit: automat.txt): ");
        string filename = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(filename))
        {
            filename = "automat.txt";
        }

        try
        {
            File.WriteAllText(filename, dfa.PrintAutomaton());
            Console.WriteLine($"✓ Automatul a fost salvat în fișierul '{filename}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Eroare la salvarea fișierului: {ex.Message}");
        }
    }

    static void CheckSingleWord(DeterministicFiniteAutomaton dfa)
    {
        Console.Write("Introduceți cuvântul de verificat: ");
        string word = Console.ReadLine();
        
        bool accepted = dfa.CheckWord(word);
        
        Console.WriteLine();
        if (accepted)
        {
            Console.WriteLine($"✓ Cuvântul '{word}' este ACCEPTAT de automat.");
        }
        else
        {
            Console.WriteLine($"✗ Cuvântul '{word}' este RESPINS de automat.");
        }
        Console.WriteLine();
    }

    static void CheckMultipleWords(DeterministicFiniteAutomaton dfa)
    {
        Console.WriteLine("Introduceți cuvintele de verificat (separate prin virgulă):");
        string input = Console.ReadLine();
        
        string[] words = input.Split(',');
        Console.WriteLine();
        Console.WriteLine("Rezultate:");
        Console.WriteLine(new string('-', 50));
        
        foreach (string word in words)
        {
            string trimmedWord = word.Trim();
            bool accepted = dfa.CheckWord(trimmedWord);
            string status = accepted ? "✓ ACCEPTAT" : "✗ RESPINS";
            Console.WriteLine($"{status,-15} | {trimmedWord}");
        }
        Console.WriteLine();
    }

    // Funcții helper pentru forma postfixată
    static string AddExplicitConcatenation(string regex)
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
    
    static string ConvertToPostfix(string regex)
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

    // Clasă pentru nodul arborelui sintactic
    class TreeNode
    {
        public string Value { get; set; }
        public TreeNode Left { get; set; }
        public TreeNode Right { get; set; }
        
        public TreeNode(string value)
        {
            Value = value;
        }
    }

    static TreeNode BuildSyntaxTree(string postfix)
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

    static void PrintTree(TreeNode node, string indent, bool last)
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
