public class MenuHandler
{
    private readonly DeterministicFiniteAutomaton _dfa;
    private readonly string _postfixForm;
    private readonly FileManager _fileManager;
    private readonly SyntaxTreeBuilder _treeBuilder;
    private readonly AutomatonService _automatonService;

    public MenuHandler(DeterministicFiniteAutomaton dfa, string postfixForm)
    {
        _dfa = dfa;
        _postfixForm = postfixForm;
        _fileManager = new FileManager();
        _treeBuilder = new SyntaxTreeBuilder();
        _automatonService = new AutomatonService();
    }

    public void Run()
    {
        bool running = true;
        while (running)
        {
            DisplayMenu();
            string choice = Console.ReadLine() ?? "";
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    DisplayPostfixForm();
                    break;
                case "2":
                    DisplaySyntaxTree();
                    break;
                case "3":
                    _automatonService.DisplayAutomaton(_dfa);
                    break;
                case "4":
                    SaveAutomatonToFile();
                    break;
                case "5":
                    _automatonService.CheckSingleWord(_dfa);
                    break;
                case "6":
                    _automatonService.CheckMultipleWords(_dfa);
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

    private void DisplayMenu()
    {
        Console.WriteLine("═══════════════════════════════════════════════════");
        Console.WriteLine("              MENIU PRINCIPAL");
        Console.WriteLine("═══════════════════════════════════════════════════");
        Console.WriteLine("1. Afișare formă poloneză postfixată");
        Console.WriteLine("2. Afișare arbore sintactic");
        Console.WriteLine("3. Afișare automat în consolă");
        Console.WriteLine("4. Salvare automat în fișier");
        Console.WriteLine("5. Verificare cuvânt");
        Console.WriteLine("6. Verificare multiple cuvinte");
        Console.WriteLine("0. Ieșire");
        Console.WriteLine("═══════════════════════════════════════════════════");
        Console.Write("Alegeți o opțiune: ");
    }

    private void DisplayPostfixForm()
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════╗");
        Console.WriteLine("║        FORMA POLONEAZĂ POSTFIXATĂ                ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine($"Forma postfixată: {_postfixForm}");
        Console.WriteLine();

        Console.WriteLine("Explicație:");
        Console.WriteLine("  '|' - alternare (OR)");
        Console.WriteLine("  '.' - concatenare");
        Console.WriteLine("  '*' - Kleene star (repetare)");
        Console.WriteLine();
    }

    private void DisplaySyntaxTree()
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════╗");
        Console.WriteLine("║           ARBORE SINTACTIC                        ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════╝");
        Console.WriteLine();

        var tree = _treeBuilder.BuildFromPostfix(_postfixForm);
        _treeBuilder.PrintTree(tree);
        Console.WriteLine();
    }

    private void SaveAutomatonToFile()
    {
        Console.Write("Introduceți numele fișierului de ieșire (implicit: automat.txt): ");
        string filename = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(filename))
        {
            filename = "automat.txt";
        }

        _fileManager.SaveToFile(filename, _dfa.PrintAutomaton());
    }
}
